using System;
using EQuery.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EQuery.Test
{
    [TestClass]
    public class TypeHelperTest
    {
        [TestMethod]
        public void TestVariousTypes()
        {
            Assert.IsTrue(TypeHelper.IsSqlSupportedType(typeof(string)));
            Assert.IsTrue(TypeHelper.IsSqlSupportedType(typeof(int)));
            Assert.IsTrue(TypeHelper.IsSqlSupportedType(typeof(bool)));
            Assert.IsTrue(TypeHelper.IsSqlSupportedType(typeof(double)));
            Assert.IsTrue(TypeHelper.IsSqlSupportedType(typeof(int?)));
            Assert.IsTrue(TypeHelper.IsSqlSupportedType(typeof(DateTime)));
            Assert.IsTrue(TypeHelper.IsSqlSupportedType(typeof(DateTime?)));
        }
    }
}
