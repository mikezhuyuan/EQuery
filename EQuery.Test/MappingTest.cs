using System;
using EQuery.Core;
using EQuery.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EQuery.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MappingTest
    {
        class Student
        {
            public int Student_id { get; set; }
            public string Name { get; set; }
            public Course Course { get; set; }
        }

        class Course
        {
            public int Course_id { get; set; }
            public string Name { get; set; }
        }        

        [TestMethod]
        public void CanGetDefaultMap()
        {
            var convention = new DefaultConvention();
            var builder = new EntityMapBuilder<Student>(convention);

            var map = builder.Build();

            Assert.IsNotNull(map);
            Assert.AreEqual("[Student]", map.Table.Value);
            Assert.AreEqual("[dbo]", map.Schema.Value);
            Assert.AreEqual("[Student_id]", map.Key.Column.Value);
            Assert.AreEqual("[Name]", map.Properties[1].Column.Value);            
        }

        [TestMethod]
        public void CanSetEntityMapping()
        {
            var convention = new DefaultConvention();
            var builder = new EntityMapBuilder<Student>(convention);

            var map = ((EntityMapBuilder<Student>)builder.Table("Students")
                            .Schema("DBO")
                            .Key(_ => _.Student_id, "ID")
                            .Property(_ => _.Name, "StudentName"))
                            .Build();

            Assert.AreEqual("[Students]", map.Table.Value);
            Assert.AreEqual("[DBO]", map.Schema.Value);
            Assert.AreEqual("[ID]", map.Key.Column.Value);
            Assert.AreEqual("[StudentName]", map.Properties[1].Column.Value);
        }

        [TestMethod]
        public void CanMapReference()
        {
            var convention = new DefaultConvention();
            var studentBuilder = new EntityMapBuilder<Student>(convention);
            var courseBuilder = new EntityMapBuilder<Course>(convention);

            var stuMap = studentBuilder.Build();
            var crsMap = courseBuilder.Build();

            studentBuilder.BuildReferences(new[] {crsMap, stuMap});

            Assert.IsNotNull("[Course_id]", stuMap.References[0].Column.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CannotMapReference()
        {
            var convention = new DefaultConvention();
            var studentBuilder = new EntityMapBuilder<Student>(convention);

            var stuMap = ((EntityMapBuilder<Student>)studentBuilder
                            .Reference(_=>_.Course, "Course_id"))
                            .Build();

            studentBuilder.BuildReferences(new[] {stuMap});
        }
    }
}
