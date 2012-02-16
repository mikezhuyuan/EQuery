using System;
using System.Collections.Generic;
using System.Reflection;
using EQuery.Core;
using EQuery.Mapping;

namespace EQuery.Sql.SqlNode
{
    class JoinGroup : ISqlNode
    {
        private readonly Dictionary<PropertyInfo, Item> _joins = new Dictionary<PropertyInfo, Item>();

        public Item FindJoin(PropertyInfo propertyInfo)
        {
            Item result;
            if (_joins.TryGetValue(propertyInfo, out result))
                return result;

            return null;            
        }

        public Item AddJoin(BracketedName fromAlias, BracketedName toAlias, ReferenceMap referenceMap)
        {
            return _joins[referenceMap.Property] = new Item(fromAlias, toAlias, referenceMap);
        }

        public void Render(SqlWriter writer)
        {
            if (_joins.Count == 0)
                return;

            foreach (var join in _joins.Values)
            {
                writer.AppendLine();
                join.Render(writer);
            }
        }

        internal class Item : ISqlNode
        {
            public BracketedName FromAlias { get; private set; }
            public BracketedName ToAlias { get; private set; }
            public ReferenceMap ReferenceMap { get; private set; }
            public string Join = SqlStrings.LeftJoin;

            public Item(BracketedName fromAlias, BracketedName toAlias, ReferenceMap referenceMap)
            {
                FromAlias = fromAlias;
                ToAlias = toAlias;
                ReferenceMap = referenceMap;
            }

            public void Render(SqlWriter writer)
            {
                writer.Append(Join, ReferenceMap.To.Table, SqlStrings.As, ToAlias, SqlStrings.NoLock);                
                writer.Indent();
                writer.AppendLine(SqlStrings.On, FromAlias + ReferenceMap.Column, "=", ToAlias + ReferenceMap.To.Key.Column);
                writer.Unindent();
            }
        }
    }
}