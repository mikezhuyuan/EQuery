using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EQuery.Core;
using EQuery.Mapping;
using EQuery.Utils;

namespace EQuery.Sql.SqlNode
{
    class Columns : ISqlNode
    {
        private readonly EntityMap _entityMap;
        private readonly BracketedName _alias;
        private bool _showRowNumber;
        private OrderBy _orderBy;

        public Columns(BracketedName alias, EntityMap entityMap)
        {
            _alias = alias;
            _entityMap = entityMap;
        }

        public void Render(SqlWriter writer)
        {
            for (var i = 0; i < _entityMap.Properties.Length; i++)
            {
                if (i > 0)
                    writer.Append(",");

                writer.Append(_alias + _entityMap.Properties[i].Column);
            }

            RenderRowNumber(writer);
        }

        public void AddRowNumber(OrderBy orderBy)
        {
            _orderBy = orderBy;
            _showRowNumber = true;
        }

        private void RenderRowNumber(SqlWriter writer)
        {
            if (!_showRowNumber)
                return;

            writer.Append(",", SqlStrings.ROW_NUMBER + "()", SqlStrings.Over, "(");

            if (_orderBy == null)
                writer.Append(SqlStrings.OrderBy, _alias + _entityMap.Key.Column, SqlStrings.Ascending);
            else
                writer.Append(_orderBy);

            writer.Append(")", SqlStrings.As, (BracketedName)SqlStrings.ROW_NUMBER);
        }
    }
}
