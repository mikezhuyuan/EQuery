using System.Collections.Generic;
using EQuery.Core;
using EQuery.Mapping;
using EQuery.Sql;

namespace EQuery
{
    public abstract class QueryFactory
    {
        private readonly MappingBuilder _builder = new MappingBuilder();
        private IEnumerable<EntityMap> _entities;

        public IQuery CreateQuery(string connectionString)
        {
            if(_entities == null)
            {
                OnMapping(_builder);
                _entities = _builder.Build();
            }

            return new Query(_entities, connectionString);
        }

        protected abstract void OnMapping(IMappingBuilder builder);
    }
}
