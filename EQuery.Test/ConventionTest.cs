using System;
using System.Reflection;
using EQuery.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EQuery.Test
{
    [TestClass]
    public class ConventionTest
    {
        public class Class0
        {
            public int Class0Id { get; set; }
            public string Name { get; set; }
        }

        public class Class1
        {
            public int Class1_Id { get; set; }
        }

        public class Class2
        {
            public int ID { get; set; }
        }

        public class Class3
        {
            public int id { get; set; }
        }

        public class Class4
        {
        }

        [TestMethod]
        public void CanGetKey()
        {
            var convention = new DefaultConvention();
            PropertyInfo prop;
            string colName;
            convention.GetKey(typeof(Class0), out prop, out colName);
            
            Assert.IsNotNull(prop);
            Assert.AreEqual("Class0Id", colName);

            convention.GetKey(typeof(Class1), out prop, out colName);
            Assert.IsNotNull(prop);
            Assert.AreEqual("Class1_Id", colName);

            convention.GetKey(typeof(Class2), out prop, out colName);
            Assert.IsNotNull(prop);
            Assert.AreEqual("ID", colName);

            convention.GetKey(typeof(Class3), out prop, out colName);
            Assert.IsNotNull(prop);
            Assert.AreEqual("id", colName);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CannotGetKey()
        {
            var convention = new DefaultConvention();
            PropertyInfo prop;
            string colName;
            convention.GetKey(typeof(Class4), out prop, out colName);
        }

        [TestMethod]
        public void CanGetTableName()
        {
            var convention = new DefaultConvention();

            Assert.AreEqual("Class0", convention.GetTableName(typeof(Class0)));
        }

        [TestMethod]
        public void CanGetColumnName()
        {
            var convention = new DefaultConvention();

            Assert.AreEqual("Name", convention.GetColumnName(typeof(Class0).GetProperty("Name")));
        }
    }
}
