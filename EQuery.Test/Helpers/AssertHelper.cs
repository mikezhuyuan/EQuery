using System.Linq;
using System.Text.RegularExpressions;
using EQuery.Core;
using EQuery.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EQuery.Test.Helpers
{
    static class AssertHelper
    {        
        public static string RemoveSpaces(string str)
        {
            return Regex.Replace(str, @"\s", string.Empty);
        }

        public static void AreEqualWithNoSpaces(string expected, string actual)
        {
            Assert.AreEqual(RemoveSpaces(expected), RemoveSpaces(actual));
        }

        public static QueryContext CreateContext<T>(IQuery query, IQueryable<T> source)
        {
            var context = ((Query)query).CreateContext(typeof(T), source.Expression);
            return context;
        }
    }
}
