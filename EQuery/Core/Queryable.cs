using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EQuery.Utils;

namespace EQuery.Core
{
    class Queryable<T> : IOrderedQueryable<T>
    {
        public Queryable(IQueryProvider provider, Expression expr)
        {
            Provider = provider;
            Expression = expr;
        }

        public Queryable(IQueryProvider provider)
        {
            Provider = provider;
            Expression = Expression.Constant(this);
        }

        public IEnumerator<T> GetEnumerator()
        {            
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public Expression Expression
        {
            get;
            private set;
        }

        public IQueryProvider Provider
        {
            get;
            private set;
        }
    }
}
