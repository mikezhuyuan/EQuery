using System;
using System.Collections.Generic;
using System.Linq;
using EQuery.Core;

namespace EQuery.Mapping
{
    class MappingBuilder : IMappingBuilder
    {
        private readonly List<Action<List<EntityMap>>> _buildEntities = new List<Action<List<EntityMap>>>();
        private readonly List<Action<List<EntityMap>>> _buildReferences = new List<Action<List<EntityMap>>>();

        public IEntityMapBuilder<T> Entity<T>() where T : class, new()
        {
            var type = typeof(T);
            var builder = new EntityMapBuilder<T>();

            _buildEntities.Add(entities =>
            {
                if (entities.Any(_ => _.Type == type))
                    throw new Exception("Already mapped : " + type);

                if (!(type.IsPublic || type.IsNested && type.IsNestedPublic))
                    throw new Exception("Type must be public : " + type);

                var entityMap = builder.Build();
                entities.Add(entityMap);

                //build assemblers
                entityMap.Assemble = ObjectAssembler.CreateAssembler<T>(entityMap);
            });

            _buildReferences.Add(entities => builder.BuildReferences(entities));

            return builder;
        }

        internal IEnumerable<EntityMap> Build()
        {
            var entities = new List<EntityMap>();

            foreach (var action in _buildEntities)
            {
                action(entities);
            }

            foreach (var action in _buildReferences)
            {
                action(entities);
            }

            return entities;
        }
    }
}