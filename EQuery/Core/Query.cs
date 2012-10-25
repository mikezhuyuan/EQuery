using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EQuery.Mapping;
using EQuery.Sql;
using EQuery.Sql.SqlNode;
using EQuery.Utils;

namespace EQuery.Core
{
    class Query : IQuery, IQueryProvider
    {
        private readonly IEnumerable<EntityMap> _entities;
        private readonly string _connectionString;

        public Query(IEnumerable<EntityMap> entities, string connectionString)
        {
            _entities = entities;
            _connectionString = connectionString;
            
        }

        public IQueryable<T> All<T>()
        {            
            return new Queryable<T>(this);
        }

        public T Get<T>(object id)
        {
            var context = new QueryContext(_entities, _connectionString) {EntityType = typeof (T)};

            var select = new Select(context);
            select.Where = 
                new Where(
                    new Equal
                    {
                        Left = new PropertyAccess{Alias = select.Alias, PropertyMap = select.EntityMap.Key},
                        Right = new Parameter(new ConstantProvider(context, id)),
                    });

            using (Profiler.Watch("Rendering"))
            {
                select.Render(context.Writer);
            }

            return new QueryExecutor(context).InnerExecuteSingle<T>();
        }

        public TRef LoadReference<TSource, TRef>(TSource obj, Expression<Func<TSource, TRef>> expr) where TRef : class
        {
            var context = new QueryContext(_entities, _connectionString) {EntityType = typeof (TSource)};

            var prop = ExpressionHelper.GetPropertyInfo(expr);
            var reference = context.EntityMap.FindReferenceMap(prop);
            if (reference == null)
                throw new Exception(prop + " not found");

            var id = context.EntityMap.Key.Property.GetValue(obj, null);
            var select = new Select(context);

            var alias = context.NextAlias();
            select.JoinGroup.AddJoin(select.Alias, alias, reference).Join = SqlStrings.Join;            

            select.Columns = new Columns(alias, reference.To);
            select.Where =
                new Where(
                    new Equal
                    {
                        Left = new PropertyAccess { Alias = select.Alias, PropertyMap = select.EntityMap.Key },
                        Right = new Parameter(new ConstantProvider(context, id)),
                    });

            using (Profiler.Watch("Rendering"))
            {
                select.Render(context.Writer);
            }

            return new QueryExecutor(context).InnerExecuteSingle<TRef>();
        }

        public IQueryable<TResult> LoadCollection<TSource, TResult>(TSource obj, Expression<Func<TSource, IEnumerable<TResult>>> expr) where TResult : class
        {
            throw new NotImplementedException();
        }

        public QueryContext CreateContext(Type type, Expression expression)
        {
            var context = new QueryContext(_entities, _connectionString) {EntityType = type};

            var converter = new QueryConverter(context);

            ISqlNode sqlNode;

            using (Profiler.Watch("Parsing"))
            {
                sqlNode = converter.Visit(expression);
            }

            using (Profiler.Watch("Rendering"))
            {
                sqlNode.Render(context.Writer);
            }

            return context;
        }

        #region IQueryProvider
        T IQueryProvider.Execute<T>(Expression expression)
        {
            var type = typeof(T);
            if(type.IsGenericType)
                type = type.GetGenericArguments()[0];
            var context = CreateContext(type, expression);

            return new QueryExecutor(context).Execute<T>();
        }

        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            return new Queryable<T>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            throw new Exception("Use generic CreateQuery");
        }

        object IQueryProvider.Execute(Expression expression)
        {
            throw new Exception("Use generic Execute");
        }
        #endregion
    }
}
