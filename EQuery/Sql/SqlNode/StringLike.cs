using EQuery.Core;

namespace EQuery.Sql.SqlNode
{
    class StringLike : IExprNode
    {        
        public QueryContext Context;
        public Parameter Parameter;
        public IExprNode PropertyAccess;
        public bool PartialPrefix;
        public bool PartialPostfix;

        public void Render(SqlWriter writer)
        {
            var value = (string)Context.Values[Parameter.Name].Value;

            if (PartialPrefix && !value.EndsWith("%"))
                value = value + '%';

            if (PartialPostfix && !value.StartsWith("%"))
                value = '%' + value;

            Context.Values[Parameter.Name].Value = value;

            PropertyAccess.Render(writer);
            writer.Append(SqlStrings.Like);
            Parameter.Render(writer);
        }

        public Precedence Precedence
        {
            get { return Precedence.High; }
        }
    }
}