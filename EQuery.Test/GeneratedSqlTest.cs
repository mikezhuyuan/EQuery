using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EQuery.Core;
using EQuery.Sql;
using EQuery.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EQuery.Test
{
    [TestClass]
    public class GeneratedSqlTest
    {
        public class Student
        {
            public int Student_id { get; set; }
            public string Name { get; set; }
            public Course Course { get; set; }
        }

        public class Course
        {
            public int Course_id { get; set; }
            public string Name { get; set; }
            public School School { get; set; }
        }

        public class School
        {
            public int School_id { get; set; }
            public string Name { get; set; }
        }

        class Factory : QueryFactory
        {
            protected override void OnMapping(IMappingBuilder builder)
            {
                builder.Entity<Student>();
                builder.Entity<Course>();
                builder.Entity<School>();
            }
        }

        [TestMethod]
        public void EntityRelationAccessInWhere()
        {
            var factory = new Factory();
            var query = (Query)factory.CreateQuery(string.Empty);
            var stus = query.All<Student>()
                            .Where(_ => _.Course.School.Name.Contains("EF"));

            var context = AssertHelper.CreateContext(query, stus);

            string expected =
@"SELECT [t0].[Student_id], [t0].[Name]
FROM [dbo].[Student] AS [t0] (NOLOCK)
LEFT JOIN [Course] AS [t1] (NOLOCK)
	ON [t0].[Course_id] = [t1].[Course_id]
LEFT JOIN [School] AS [t2] (NOLOCK)
	ON [t1].[School_id] = [t2].[School_id]
WHERE [t2].[Name] LIKE @p0";

            var result = context.Writer.GetResult();
            AssertHelper.AreEqualWithNoSpaces(expected, result);
            Assert.AreEqual("%EF%", context.Values["@p0"].Value);
        }

        [TestMethod]
        public void ReferenceNullInWhere()
        {
            var factory = new Factory();
            var query = (Query)factory.CreateQuery(string.Empty);
            var courses = query.All<Course>()
                            .Where(_ => _.School != null);

            var context = AssertHelper.CreateContext(query, courses);
            var result = context.Writer.GetResult();
            
            string expected =
@"SELECT
	[t0].[Course_id] , [t0].[Name]
FROM [dbo].[Course] AS [t0] (NOLOCK)
WHERE [t0].[School_id] IS NOT NULL";

            AssertHelper.AreEqualWithNoSpaces(expected, result);
        }

        [TestMethod]
        public void ManyParameters()
        {
            var factory = new Factory();
            var query = (Query)factory.CreateQuery(string.Empty);
            var stus = query.All<Student>()
                            .Where(_ => _.Student_id == 1 && _.Student_id == 1 && _.Student_id == 1 && _.Student_id == 1 && _.Student_id == 1 && _.Student_id == 1 && _.Student_id == 1);

            var context = AssertHelper.CreateContext(query, stus);

            Assert.AreEqual(7, context.Values.Count);
        }

        [TestMethod]
        public void TestOrderBy()
        {
            var factory = new Factory();
            var query = (Query)factory.CreateQuery(string.Empty);
            var stus = query.All<Student>()
                            .OrderBy(_ => _.Name)
                            .ThenByDescending(_ => _.Course.Name);
            var context = AssertHelper.CreateContext(query, stus);            
            var result = context.Writer.GetResult();
            var expected =
@"SELECT [t0].[Student_id], [t0].[Name]
FROM [dbo].[Student] AS [t0] (NOLOCK)
LEFT JOIN [Course] AS [t1] (NOLOCK)
	ON [t0].[Course_id] = [t1].[Course_id]
ORDER BY [t0].[Name] ASC, [t1].[Name] DESC";

            AssertHelper.AreEqualWithNoSpaces(expected, result);
        }

        [TestMethod]
        public void OrderByReference()
        {
            var factory = new Factory();
            var query = (Query)factory.CreateQuery(string.Empty);
            var stus = query.All<Student>()
                .OrderBy(_ => _.Course);
            var context = AssertHelper.CreateContext(query, stus);
            var result = context.Writer.GetResult();
            var expected =
@"SELECT [t0].[Student_id], [t0].[Name]
FROM [dbo].[Student] AS [t0] (NOLOCK)
ORDER BY [t0].[Course_id] ASC";

            AssertHelper.AreEqualWithNoSpaces(expected, result);
        }

        [TestMethod]
        public void TestTake()
        {
            var factory = new Factory();
            var query = (Query)factory.CreateQuery(string.Empty);
            var stus = query.All<Student>()
                            .Take(10);
            var context = AssertHelper.CreateContext(query, stus);
            var result = context.Writer.GetResult();
            var expected =
@"SELECT TOP(@p0) [t0].[Student_id], [t0].[Name]
FROM [dbo].[Student] AS [t0] (NOLOCK)";

            AssertHelper.AreEqualWithNoSpaces(expected, result);            
        }

        [TestMethod]
        public void TestSkip()
        {
            var factory = new Factory();
            var query = (Query)factory.CreateQuery(string.Empty);
            var stus = query.All<Student>()
                            .OrderBy(_=>_.Name)
                            .Skip(10);
            var context = AssertHelper.CreateContext(query, stus);
            var result = context.Writer.GetResult();
            var expected =
@"SELECT
*
FROM(
    SELECT
        [t0].[Student_id], [t0].[Name], ROW_NUMBER() OVER ( ORDER BY [t0].[Name] ASC) AS [ROW_NUMBER] 
    FROM [dbo].[Student] AS [t0] (NOLOCK)
) AS [t1]
WHERE [ROW_NUMBER]>@p0";

            AssertHelper.AreEqualWithNoSpaces(expected, result);  
        }

        [TestMethod]
        public void TestSkipWithoutOrder()
        {
            var factory = new Factory();
            var query = (Query)factory.CreateQuery(string.Empty);
            var stus = query.All<Student>()                            
                            .Skip(10)
                            .Take(10);
            var context = AssertHelper.CreateContext(query, stus);
            var result = context.Writer.GetResult();
            var expected =
@"SELECT
TOP(@p1)
*
FROM(
    SELECT
        [t0].[Student_id], [t0].[Name], ROW_NUMBER() OVER ( ORDER BY [t0].[Student_id] ASC) AS [ROW_NUMBER] 
    FROM [dbo].[Student] AS [t0] (NOLOCK)
) AS [t1]
WHERE [ROW_NUMBER]>@p0";

            AssertHelper.AreEqualWithNoSpaces(expected, result);
        }
    }
}
