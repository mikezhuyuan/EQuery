using System;
using System.Collections.Generic;
using EQuery.Mapping;
using EQuery.Utils;

namespace EQuery.Core
{
    class QueryContext
    {
        private int _alias;
        private int _param;
        private SqlWriter _writer;

        public QueryContext(IEnumerable<EntityMap> entityMaps, string connectionString)
        {
            if (entityMaps == null)
                throw new ArgumentNullException("entityMaps");

            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            Values = new Dictionary<string, IValueProvider>();
            EntityMaps = entityMaps;            
            ConnectionString = connectionString;

            _writer = new SqlWriter();
            _alias = 0;
            _param = 0;
        }

        public EntityMap GetEntityMap(Type type)
        {
            foreach (var map in EntityMaps)
                if (map.Type == type)
                    return map;

            return null;
        }

        public EntityMap EntityMap
        {
            get
            {
                if (EntityType == null)
                    throw new NullReferenceException("EntityType");

                return GetEntityMap(EntityType);
            }
        }

        public string NextAlias()
        {
            return "t" + (_alias++);
        }

        public string NextParam(IValueProvider value)
        {            
            var p = "@p" + (_param++);

            Values[p] = value;

            return p;
        }

        public IDictionary<string, IValueProvider> Values
        {
            get;
            private set;
        }

        public SqlWriter Writer
        {
            get { return _writer; }
        }

        public IEnumerable<EntityMap> EntityMaps
        {
            get;
            private set;
        }

        public Type EntityType { get; set; }

        public string ConnectionString { get; private set; }
    }
}