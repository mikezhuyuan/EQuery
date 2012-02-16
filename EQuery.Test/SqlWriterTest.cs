using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EQuery.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EQuery.Test
{
    [TestClass]
    public class SqlWriterTest
    {
        [TestMethod]
        public void CanWriteFormattedText()
        {
            string expected = "<ul>\n\t<li>\n\t\t1 + 2 = 3\n\t</li>\n</ul>";

            var writer = new SqlWriter();
            writer.AppendLine("<ul>");            
                writer.Indent();
                    writer.AppendLine("<li>");
                        writer.Indent();
                            writer.AppendLine("1", "+", "2", "=", "3");
                        writer.Unindent();
                    writer.AppendLine("</li>");            
                writer.Unindent();
            writer.AppendLine("</ul>");

            var result = writer.GetResult();
            Assert.AreEqual(expected, result);
        }
    }
}
