using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EQuery
{
    public interface IQuery
    {
        IQueryable<T> All<T>();
        T Get<T>(object id);
        TRef LoadReference<TSource, TRef>(TSource obj, Expression<Func<TSource, TRef>> expr) where TRef : class;
        IQueryable<TResult> LoadCollection<TSource, TResult>(TSource obj, Expression<Func<TSource, IEnumerable<TResult>>> expr) where TResult : class;
    }
}