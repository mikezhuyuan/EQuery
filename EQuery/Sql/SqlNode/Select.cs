using System;
using System.Collections.Generic;
using System.Reflection;
using EQuery.Core;
using EQuery.Mapping;
using EQuery.Utils;

namespace EQuery.Sql.SqlNode
{
    interface ISelect : ISqlNode
    {
        Top Top { get; set; }
    }

    class Select : ISelect
    {
        public readonly BracketedName Alias;
        public readonly EntityMap EntityMap;

        public Top Top { get; set; }

        public Columns Columns;

        public JoinGroup JoinGroup;

        public Where Where;

        public From From;

        public OrderBy OrderBy;

        public Select(QueryContext context) : this(context.GetEntityMap(context.EntityType), context.NextAlias())
        {
            
        }

        public Select(EntityMap entityMap, BracketedName alias)
        {
            if (entityMap == null)
                throw new ArgumentNullException("entityMap");

            if (alias == null)
                throw new ArgumentNullException("alias");

            EntityMap = entityMap;
            Alias = alias;

            JoinGroup = new JoinGroup();
            Columns = new Columns(alias, entityMap);
            From = new From(this);            
        }

        public void Render(SqlWriter writer)
        {
            writer.AppendLine(SqlStrings.Select);

            writer.Indent();

            writer.AppendLine(Top);

            writer.AppendLine(Columns);

            writer.Unindent();

            writer.AppendLine(From);

            writer.AppendLine(JoinGroup);

            writer.AppendLine(Where);

            writer.AppendLine(OrderBy);
        }

        public bool VisitPath(QueryContext context, PropertyInfo[] path, out BracketedName alias, out PropertyMap map)
        {
            var entity = EntityMap;
            alias = Alias;
            map = null;

            for (var i = 0; i < path.Length - 1; i++)
            {
                var prop = path[i];
                var item = JoinGroup.FindJoin(prop);
                if (item == null)
                {
                    var reference = entity.FindReferenceMap(prop);

                    if (reference == null)
                        return false;

                    item = JoinGroup.AddJoin(alias, context.NextAlias(), reference);
                }

                alias = item.ToAlias;
                entity = item.ReferenceMap.To;
            }

            var last = path[path.Length - 1];
            map = entity.FindPropertyMap(last) ?? entity.FindReferenceMap(last);

            return true;
        }
    }
}