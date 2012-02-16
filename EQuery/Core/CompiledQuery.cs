using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EQuery.Utils;

namespace EQuery.Core
{
    static class CompiledQuery
    {
        public static Func<IEnumerable<TResult>> Compile<TResult>(IQueryable<TResult> source, Expression<Func<IQueryable<TResult>, IQueryable<TResult>>> expr)
        {
            if (source.Provider is Query)
            {
                QueryContext context = GetContext(source, expr);

                return () => new QueryExecutor(context).Execute<IEnumerable<TResult>>();
            }

            return null;
        }

        public static Func<T1, IEnumerable<TResult>> Compile<T1, TResult>(IQueryable<TResult> source, Expression<Func<IQueryable<TResult>, T1, IQueryable<TResult>>> expr)
        {
            if (source.Provider is Query)
            {
                QueryContext context = GetContext(source, expr);
                var replace = ParameterReplacement(context, expr);

                return (p1) =>
                {
                    replace(1, p1);

                    return new QueryExecutor(context).Execute<IEnumerable<TResult>>();
                };
            }

            return null;
        }

        public static Func<T1, T2, IEnumerable<TResult>> Compile<T1, T2, TResult>(IQueryable<TResult> source, Expression<Func<IQueryable<TResult>, T1, T2, IQueryable<TResult>>> expr)
        {
            if (source.Provider is Query)
            {
                QueryContext context = GetContext(source, expr);
                var replace = ParameterReplacement(context, expr);

                return (p1, p2) =>
                {
                    replace(1, p1);
                    replace(2, p2);

                    return new QueryExecutor(context).Execute<IEnumerable<TResult>>();
                };
            }

            return null;
        }

        public static Func<T1, T2, T3, T4, IEnumerable<TResult>> Compile<T1, T2, T3, T4, TResult>(IQueryable<TResult> source, Expression<Func<IQueryable<TResult>, T1, T2, T3, T4, IQueryable<TResult>>> expr)
        {
            if (source.Provider is Query)
            {
                QueryContext context = GetContext(source, expr);
                var replace = ParameterReplacement(context, expr);

                return (p1, p2, p3, p4) =>
                {
                    replace(1, p1);
                    replace(2, p2);
                    replace(3, p3);
                    replace(4, p4);

                    return new QueryExecutor(context).Execute<IEnumerable<TResult>>();
                };
            }

            return null;
        }

        public static Func<T1, T2, T3, T4, T5, IEnumerable<TResult>> Compile<T1, T2, T3, T4, T5, TResult>(IQueryable<TResult> source, Expression<Func<IQueryable<TResult>, T1, T2, T3, T4, T5, IQueryable<TResult>>> expr)
        {
            if (source.Provider is Query)
            {
                QueryContext context = GetContext(source, expr);
                var replace = ParameterReplacement(context, expr);
                return (p1, p2, p3, p4, p5) =>
                {
                    replace(1, p1);
                    replace(2, p2);
                    replace(3, p3);
                    replace(4, p4);
                    replace(5, p5);

                    return new QueryExecutor(context).Execute<IEnumerable<TResult>>();
                };
            }

            return null;
        }

        private static QueryContext GetContext<TResult>(IQueryable<TResult> source, LambdaExpression expr)
        {
            var parameters = expr.Parameters;
            var body = expr.Body;
            var combinedExpr = new ExpressionModifier(parameters[0], source.Expression).Visit(body);
                
            var query = (Query)source.Provider;
            var context = query.CreateContext(typeof(TResult), combinedExpr);            

            return context;
        }

        private static Action<int, object> ParameterReplacement(QueryContext context, LambdaExpression expr)
        {
            var parameters = new List<ParameterProvider>();
            var paramExprs = expr.Parameters;

            foreach (var p in context.Values)
            {
                if (p.Value is ParameterProvider)
                {
                    var parameter = (ParameterProvider)p.Value;
                    for (int i = 0; i < paramExprs.Count; i++)
                    {
                        var name = paramExprs[i].Name;
                        if (parameter.ExpressionName == name)
                        {
                            parameter.Ordinal = i;
                            parameters.Add(parameter);
                        }
                    }
                }
            }

            return (index, value) =>
            {
                foreach (var p in parameters)
                    if (p.Ordinal == index)
                        p.Value = value;
            };
        }
    }
}
