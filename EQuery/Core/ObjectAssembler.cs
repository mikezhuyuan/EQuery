using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.SqlClient;
using EQuery.Mapping;

namespace EQuery.Core
{ 
    static class ObjectAssembler
    {
        public static Func<SqlDataReader,T> CreateAssembler<T>(EntityMap entityMap)
        {
            var props = entityMap.Properties;

            var dr = Expression.Parameter(typeof(SqlDataReader), "dr");
            var itm = Expression.Parameter(entityMap.Type, "itm");

            Expression body = Expression.Block(
                                    new[] { itm },
                                    Expression.Assign(itm, Expression.New(entityMap.Type))
                                    .Union(props.Select((prop, index) =>
                                    {
                                        var name = prop.Property.Name;
                                        var type = prop.Property.PropertyType;
                                        var method = typeof(ObjectAssembler).GetMethod("GetValue").MakeGenericMethod(type);

                                        return Expression.Assign(
                                               Expression.Property(itm, name),
                                               Expression.Call(null, method, dr, Expression.Constant(index)));
                                    }))
                                    .Union(itm));

            return Expression.Lambda<Func<SqlDataReader, T>>(body, new[] { dr }).Compile();
        }

        public static T GetValue<T>(SqlDataReader dr, int ordinal)
        {
            if (dr.IsDBNull(ordinal))
                return default(T);

            return (T)dr.GetValue(ordinal);
        }

        private static IEnumerable<Expression> Union(this Expression exp, IEnumerable<Expression> list)
        {
            return new[] { exp }.Union(list);
        }

        private static IEnumerable<Expression> Union(this IEnumerable<Expression> list, Expression exp)
        {
            return list.Union(new[] { exp });
        }
    }
}