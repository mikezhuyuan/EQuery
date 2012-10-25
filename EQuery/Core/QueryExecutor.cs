using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;
using EQuery.Utils;

namespace EQuery.Core
{
    class QueryExecutor
    {
        private QueryContext _context;
        private static Dictionary<Type, SqlDbType> _dbTypeMap;
        static QueryExecutor()
        {
            _dbTypeMap = new Dictionary<Type, SqlDbType>
            {
                {typeof(byte), GetDBType(typeof(byte))},
                {typeof(bool), GetDBType(typeof(bool))},
                {typeof(char), GetDBType(typeof(char))},
                {typeof(int), GetDBType(typeof(int))},
                {typeof(short), GetDBType(typeof(short))},    
                {typeof(long), GetDBType(typeof(long))},
                {typeof(float), SqlDbType.Real},                            
                {typeof(double), SqlDbType.Float},
                {typeof(decimal), SqlDbType.Decimal},
                {typeof(DateTime), GetDBType(typeof(DateTime))},
                {typeof(byte?), GetDBType(typeof(byte?))},
                {typeof(bool?), GetDBType(typeof(bool?))},
                {typeof(char?), GetDBType(typeof(char?))},
                {typeof(int?), GetDBType(typeof(int?))},
                {typeof(short?), GetDBType(typeof(short?))},    
                {typeof(long?), GetDBType(typeof(long?))},
                {typeof(float?), SqlDbType.Real},                            
                {typeof(double?), SqlDbType.Float},
                {typeof(decimal?), SqlDbType.Decimal},
                {typeof(DateTime?), GetDBType(typeof(DateTime?))},
            };
        }

        public QueryExecutor(QueryContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        public TCollection Execute<TCollection>()
        {
            return ExecuteCache<TCollection>.Exec(this);
        }

        internal IEnumerable<T> InnerExecuteMany<T>()
        {
            var result = new List<T>();
            InnerExecute<T>((reader, assemble) =>
            {
                while (reader.Read())
                {
                    result.Add(assemble(reader));
                }                
            });

            return result;
        }

        internal T InnerExecuteSingle<T>()
        {
            var result = default(T);
            InnerExecute<T>((reader, assemble) =>
            {
                if (reader.Read())
                    result = assemble(reader);                
            });

            return result;
        }

        private void InnerExecute<T>(Action<SqlDataReader, Func<SqlDataReader, T>> action)
        {
            var type = typeof(T);
            var entityMap = _context.GetEntityMap(type);
            if (entityMap == null)
                throw new NullReferenceException("Type not found: " + type);

            var assemble = (Func<SqlDataReader, T>)entityMap.Assemble;

            var sql = _context.Writer.GetResult();

            using (var connection = new SqlConnection(_context.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                PrepareParams(command);

                Profiler.Watch("ExecuteReader");
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                Profiler.EndWatch("ExecuteReader");

                using (Profiler.Watch("Assembling"))
                using (reader)
                {
                    action(reader, assemble);                    
                }
            }
        }

        private void PrepareParams(SqlCommand command)
        {
            foreach (var pair in _context.Values)
            {
                var value = ((IValueProvider)pair.Value).Value;
                var type = value.GetType();
               
                if(type == typeof(string))
                    command.Parameters.Add(pair.Key, SqlDbType.NVarChar, 4000).Value = value;                    
                else
                    command.Parameters.Add(pair.Key, _dbTypeMap[type]).Value = value;
            }
        }

        //http://windev.wordpress.com/2007/01/01/utility-function-to-convert-type-to-sqldbtype/
        private static SqlDbType GetDBType(Type theType)
        {
            SqlParameter param;
            System.ComponentModel.TypeConverter tc;
            param = new SqlParameter();
            tc = System.ComponentModel.TypeDescriptor.GetConverter(param.DbType);
            if (tc.CanConvertFrom(theType))
            {
                param.DbType = (DbType)tc.ConvertFrom(theType.Name);
            }
            else
            {
                // try to forcefully convert
                try
                {
                    param.DbType = (DbType)tc.ConvertFrom(theType.Name);
                }
                catch (Exception e)
                {
                    // ignore the exception
                }
            }
            return param.SqlDbType;
        }

        private static class ExecuteCache<T>
        {
            public static Func<QueryExecutor, T> Exec;

            static ExecuteCache()
            {
                Type type = typeof(T);

                var method = type.IsGenericType
                             ? typeof(QueryExecutor).GetMethod("InnerExecuteMany", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(type.GetGenericArguments()[0])
                             : typeof(QueryExecutor).GetMethod("InnerExecuteSingle", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(type);

                var obj = Expression.Parameter(typeof(QueryExecutor), "obj");

                var body = Expression.Call(obj, method);

                Exec = Expression.Lambda<Func<QueryExecutor, T>>(body, new[] { obj }).Compile();
            }
        }
    }
}
