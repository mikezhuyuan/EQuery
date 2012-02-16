using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EQuery.Core;
using EQuery.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EQuery.Test
{
    [TestClass]
    public class BracketedNameTest
    {
        [TestMethod]
        public void CanParseString()
        {
            BracketedName name = "dbo";

            Assert.AreEqual("[dbo]", name.Value);
            Assert.AreEqual("dbo", name.Raw);
            
            name = "[dbo]";

            Assert.AreEqual("[dbo]", name.Value);
            Assert.AreEqual("dbo", name.Raw);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FailIfInvalidName()
        {
            BracketedName name = "dbo]";
        }
    }
}
