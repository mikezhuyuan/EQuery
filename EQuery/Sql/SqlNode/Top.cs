using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EQuery.Core;

namespace EQuery.Sql.SqlNode
{    
    class Top : ISqlNode
    {
        private readonly IValueProvider _valueProvider;

        public Top(IValueProvider valueProvider)
        {
            _valueProvider = valueProvider;
        }

        public void Render(SqlWriter writer)
        {
            writer.Append(SqlStrings.Top + '(' + _valueProvider.ParamName + ')');
        }
    }
}
