using System;
using System.Linq.Expressions;
using EQuery.Core;
using EQuery.Mapping;
using EQuery.Sql;
using EQuery.Sql.SqlNode;
using EQuery.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EQuery.Test
{
    [TestClass]
    public class WhereTest
    {
        class School
        {
            public int School_id { get; set; }
            public string Name { get; set; }            
        }

        class Student
        {
            public int ID { get; set; }
            public int? No { get; set; }            
            public string Name { get; set; }
            public DateTime DOB { get; set; }
            public Decimal Fee { get; set; }
            public bool IsActive { get; set; }

            public School School { get; set; }
        }

        static string GetWhere(Expression<Func<Student, bool>> expr)
        {
            var convention = new DefaultConvention();
            var builder = new EntityMapBuilder<Student>(convention);
            
            var map = ((EntityMapBuilder<Student>)builder
                        .Key(_ => _.ID, "Student_id"))
                        .Build();

            var context = new QueryContext(new[]{map}, "");
            var selectEntity = new Select(map, "t0");
            var whereParser = new WhereConverter(selectEntity, context);

            var node = whereParser.Visit(expr);
            node.Render(context.Writer);
            var result = context.Writer.GetResult();

            return result;
        }

        [TestMethod]
        public void RelationalTest()
        {
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Name] = @p0", GetWhere(_ => _.Name == "mike"));
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Student_id] < @p0", GetWhere(_ => _.ID < 1));
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Student_id] >= @p0", GetWhere(_ => _.ID >= 1));
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Student_id] <> @p0", GetWhere(_ => _.ID != 1));

            string name = "mike";
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Name] <> @p0", GetWhere(_ => _.Name != name));

            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Name] IS NULL", GetWhere(_ => _.Name == null));

            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Name] IS NOT NULL", GetWhere(_ => _.Name != null));
        }

        [TestMethod]
        public void ConditionalTest()
        {
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Student_id] = @p0 AND [t0].[Student_id] = @p1", GetWhere(_ => _.ID == 1 && _.ID == 1));
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Student_id] = @p0 OR [t0].[Student_id] = @p1", GetWhere(_ => _.ID == 1 || _.ID == 1));
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Student_id] = @p0 AND [t0].[Student_id] = @p1 OR [t0].[Student_id] = @p2", GetWhere(_ => _.ID == 1 && _.ID == 1 || _.ID == 1));
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Student_id] = @p0 AND ([t0].[Student_id] = @p1 OR [t0].[Student_id] = @p2)", GetWhere(_ => _.ID == 1 && (_.ID == 1 || _.ID == 1)));
            AssertHelper.AreEqualWithNoSpaces("WHERE NOT [t0].[Student_id] = @p0", GetWhere(_ => !(_.ID == 1)));
            AssertHelper.AreEqualWithNoSpaces("WHERE NOT ([t0].[Student_id] = @p0 AND [t0].[Student_id] = @p1 OR [t0].[Student_id] = @p2)", GetWhere(_ => !(_.ID == 1 && _.ID == 1 || _.ID == 1)));
        }

        [TestMethod]
        public void StringLikeTest()
        {
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Name] LIKE @p0", GetWhere(_ => _.Name.Contains("123")));
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Name] LIKE @p0", GetWhere(_ => _.Name.StartsWith("123")));
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Name] LIKE @p0", GetWhere(_ => _.Name.EndsWith("123")));
        }

        [TestMethod]
        public void BooleanPropertyTest()
        {
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[IsActive] = 1", GetWhere(_ => _.IsActive));
            AssertHelper.AreEqualWithNoSpaces("WHERE NOT [t0].[IsActive] = 1", GetWhere(_ => !_.IsActive));
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[IsActive] = @p0", GetWhere(_ => _.IsActive == true));
            AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[IsActive] = @p0", GetWhere(_ => _.IsActive == false));
            AssertHelper.AreEqualWithNoSpaces("WHERE NOT NOT [t0].[IsActive] = @p0", GetWhere(_ => !!(_.IsActive == false)));
        }

        [TestMethod]
        public void FailedCases()
        {
            try
            {
                AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Name] LIKE @p0", GetWhere(_ => _.Name.Contains(null)));
                Assert.Fail();
            }
            catch { }

            try
            {
                AssertHelper.AreEqualWithNoSpaces("WHERE [t0].[Name] LIKE @p0", GetWhere(_ => _.Name.Equals("123")));
                Assert.Fail();
            }
            catch { }
        }
    }
}
