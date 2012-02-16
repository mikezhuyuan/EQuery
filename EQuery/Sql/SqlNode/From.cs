using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EQuery.Core;
using EQuery.Mapping;

namespace EQuery.Sql.SqlNode
{
    class From : ISqlNode
    {
        private readonly Select _select;

        public From(Select select)
        {
            _select = select;
        }

        public void Render(SqlWriter writer)
        {
            writer.AppendLine(SqlStrings.From, _select.EntityMap.Schema + _select.EntityMap.Table, SqlStrings.As, _select.Alias, SqlStrings.NoLock);
        }
    }
}
