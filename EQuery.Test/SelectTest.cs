using EQuery.Core;
using EQuery.Mapping;
using EQuery.Sql.SqlNode;
using EQuery.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EQuery.Test
{
    [TestClass]
    public class SelectTest
    {
        class School
        {
            public int School_id { get; set; }
            public string Name { get; set; }
        }

        class Student
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public School School { get; set; }
        }

        [TestMethod]
        public void CanRenderSelect()
        {
            var writer = new SqlWriter();
            var convention = new DefaultConvention();
            var builder = new EntityMapBuilder<Student>(convention);

            var map = ((EntityMapBuilder<Student>) builder
                                                       .Key(_ => _.ID, "Student_id")).Build();                        

            var selectEntity = new Select(map, "t0");
            
            selectEntity.Render(writer);
            var result = writer.GetResult();

            AssertHelper.AreEqualWithNoSpaces(
@"SELECT [t0].[Student_id], [t0].[Name]
FROM [dbo].[Student] AS [t0] (NOLOCK)", result);
        }
    }
}
