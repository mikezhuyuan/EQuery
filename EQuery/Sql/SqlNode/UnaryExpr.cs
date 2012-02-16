using EQuery.Core;

namespace EQuery.Sql.SqlNode
{
    abstract class UnaryExpr : IExprNode
    {
        public IExprNode Expr;
        public virtual void Render(SqlWriter writer)
        {
            writer.Append(Operator);
            if(Precedence > Expr.Precedence)
            {
                writer.Append("(");
                writer.Append(Expr);
                writer.Append(")");
            }
            else
            {                
                writer.Append(Expr);
            }
        }

        public abstract string Operator { get; }
        public abstract Precedence Precedence { get; }
    }
}