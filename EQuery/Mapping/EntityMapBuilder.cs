using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EQuery.Core;
using EQuery.Utils;

namespace EQuery.Mapping
{
    class EntityMapBuilder<TEntity> : IEntityMapBuilder<TEntity> where TEntity : class, new()
    {
        private INamingConvention _convention;
        private Type _entityType;
        private string _table;
        private string _schema;
        private PropertyInfo _key;
        private Dictionary<PropertyInfo, string> _propCols = new Dictionary<PropertyInfo, string>();
        private Dictionary<PropertyInfo, string> _refCols = new Dictionary<PropertyInfo, string>();
        private EntityMap _entityMap;

        internal EntityMapBuilder()
            : this(IoC.Resolve<INamingConvention>())
        {
            
        }

        internal EntityMapBuilder(INamingConvention convention)
        {
            _convention = convention;
            _entityType = typeof(TEntity);
        }

        public IEntityMapBuilder<TEntity> Table(string table)
        {
            _table = table;

            return this;
        }

        public IEntityMapBuilder<TEntity> Schema(string schema)
        {
            _schema = schema;

            return this;
        }

        public IEntityMapBuilder<TEntity> Key<TProperty>(Expression<Func<TEntity, TProperty>> expr, string column)
        {
            //todo: validation
            _propCols[_key = ExpressionHelper.GetPropertyInfo(expr)] = column;

            return this;
        }

        public IEntityMapBuilder<TEntity> Property<TProperty>(Expression<Func<TEntity, TProperty>> expr, string column)
        {
            //todo: validation
            _propCols[ExpressionHelper.GetPropertyInfo(expr)] = column;

            return this;
        }

        public IEntityMapBuilder<TEntity> Reference<TProperty>(Expression<Func<TEntity, TProperty>> expr, string foreignKey)
        {
            _refCols[ExpressionHelper.GetPropertyInfo(expr)] = foreignKey;

            return this;
        }

        internal EntityMap Build()
        {
            var props = _entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if(string.IsNullOrWhiteSpace(_table))
                _table = _convention.GetTableName(_entityType);

            if (string.IsNullOrWhiteSpace(_schema))
                _schema = "dbo";

            if (_key == null)
            {
                string keyCol;
                _convention.GetKey(_entityType, out _key, out keyCol);

                if(_key == null)
                    throw new ArgumentException("Cannot find primary key by convention");

                if (!_propCols.ContainsKey(_key))
                    _propCols[_key] = keyCol;
            }

            foreach(var prop in props)
            {
                if (!_propCols.ContainsKey(prop)
                    && prop != _key
                    && prop.IsEntityProperty())
                {
                    _propCols[prop] = _convention.GetColumnName(prop);
                }
            }

            return _entityMap = 
                   new EntityMap
                       {
                           Type = _entityType,
                           Key = new PropertyMap(_key, _propCols[_key]),
                           Table = new BracketedName(_table),
                           Schema = new BracketedName(_schema),
                           Properties = _propCols
                               .Select(_ => new PropertyMap(_.Key, _.Value))
                               .ToArray()
                       };
        }

        internal EntityMap BuildReferences(IEnumerable<EntityMap> entityMaps)
        {
            var props = _entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var references = new List<ReferenceMap>();

            foreach (var prop in props)
            {
                var map = entityMaps.FirstOrDefault(_ => _.Type == prop.PropertyType);
                
                if(map != null)
                {
                    if (!_refCols.ContainsKey(prop))
                    {
                        _refCols[prop] = _convention.GetForeignKeyName(prop);
                    }

                    references.Add(new ReferenceMap(prop, _refCols[prop], _entityMap, map));                                       
                }
                else if(_refCols.ContainsKey(prop))
                {
                    throw new Exception("Cannot find entity for property " + prop);
                }
            }

            _entityMap.References = references.ToArray();
            return _entityMap;
        }
    }
}