using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EQuery.Core;

namespace EQuery
{
    public static class QuerableExtention
    {
        public static Func<IEnumerable<TResult>> Compile<TResult>(this IQueryable<TResult> source, Expression<Func<IQueryable<TResult>, IQueryable<TResult>>> expr)
        {
            return CompiledQuery.Compile(source, expr);
        }

        public static Func<T1, IEnumerable<TResult>> Compile<T1, TResult>(this IQueryable<TResult> source, Expression<Func<IQueryable<TResult>, T1, IQueryable<TResult>>> expr)
        {
            return CompiledQuery.Compile(source, expr);
        }

        public static Func<T1, T2, IEnumerable<TResult>> Compile<T1, T2, TResult>(this IQueryable<TResult> source, Expression<Func<IQueryable<TResult>, T1, T2, IQueryable<TResult>>> expr)
        {
            return CompiledQuery.Compile(source, expr);
        }

        public static Func<T1, T2, T3, T4, IEnumerable<TResult>> Compile<T1, T2, T3, T4, TResult>(this IQueryable<TResult> source, Expression<Func<IQueryable<TResult>, T1, T2, T3, T4, IQueryable<TResult>>> expr)
        {
            return CompiledQuery.Compile(source, expr);
        }

        public static Func<T1, T2, T3, T4, T5, IEnumerable<TResult>> Compile<T1, T2, T3, T4, T5, TResult>(this IQueryable<TResult> source, Expression<Func<IQueryable<TResult>, T1, T2, T3, T4, T5, IQueryable<TResult>>> expr)
        {
            return CompiledQuery.Compile(source, expr);
        }
    }
}
