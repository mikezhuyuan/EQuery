using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EQuery.Core;

namespace EQuery.Sql.SqlNode
{
    class SelectPaging : ISelect
    {
        private readonly BracketedName _alias;
        private IValueProvider _skip;
        private Select _innerSelect;

        public Top Top { get; set; }

        public SelectPaging(BracketedName alias, Select innerSelect, IValueProvider skip)
        {
            innerSelect.Columns.AddRowNumber(innerSelect.OrderBy);
            innerSelect.OrderBy = null;

            _skip = skip;
            _innerSelect = innerSelect;
            _alias = alias;
        }

        public void Render(SqlWriter writer)
        {
            writer.AppendLine(SqlStrings.Select);

            writer.Indent();

            writer.AppendLine(Top);

            writer.AppendLine("*");

            writer.Unindent();

            writer.AppendLine(SqlStrings.From, "(");

            writer.Indent();

            writer.AppendLine(_innerSelect);

            writer.Unindent();

            writer.AppendLine(")", SqlStrings.As, _alias);   
            
            writer.AppendLine(SqlStrings.Where, (BracketedName)SqlStrings.ROW_NUMBER, ">", _skip.ParamName);
        }
    }
}
