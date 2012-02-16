using System.Linq.Expressions;

namespace EQuery.Utils
{
    class ExpressionModifier : ExpressionVisitor
    {
        private Expression _from;
        private Expression _to;

        public ExpressionModifier(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _from)
                return _to;

            return base.Visit(node);
        }
    }
}