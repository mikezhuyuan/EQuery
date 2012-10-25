using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace EQuery.Console
{
    public interface IStudentRepository
    {
        IEnumerable<Student> All();
    }

    class StudentRepository : IStudentRepository
    {
        private IQuery _query;
        public StudentRepository(IQuery query = null)
        {
            if (query == null)
            {
                var connStr = @"Data Source=.;Initial Catalog=test;Integrated Security=True";

                var factory = new MyQueryFactory();

                query = factory.CreateQuery(connStr);
            }

            _query = query;
        }

        public IEnumerable<Student> All()
        {
            return _query.All<Student>();
        }
    }

    class MockQuery : IQuery
    {
        private readonly IEnumerable<Student> _students;

        public MockQuery(IEnumerable<Student> students)
        {
            _students = students;
        }

        public IQueryable<T> All<T>()
        {
            return (IQueryable<T>)_students.AsQueryable();
        }

        public T Get<T>(object id)
        {
            throw new NotImplementedException();
        }

        public TRef LoadReference<TSource, TRef>(TSource obj, Expression<Func<TSource, TRef>> expr) where TRef : class
        {
            throw new NotImplementedException();
        }

        public IQueryable<TResult> LoadCollection<TSource, TResult>(TSource obj, Expression<Func<TSource, IEnumerable<TResult>>> expr) where TResult : class
        {
            throw new NotImplementedException();
        }
    }
}
