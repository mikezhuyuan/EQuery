using EQuery.Core;

namespace EQuery.Sql.SqlNode
{
    class Parameter : IExprNode
    {
        public Parameter(IValueProvider valueProvider)
        {
            _valueProvider = valueProvider;
        }

        private readonly IValueProvider _valueProvider;

        public string Name { get { return _valueProvider.ParamName; } }

        public void Render(SqlWriter writer)
        {
            writer.Append(Name);
        }

        public Precedence Precedence
        {
            get { return Precedence.Hightest; }
        }
    }
}