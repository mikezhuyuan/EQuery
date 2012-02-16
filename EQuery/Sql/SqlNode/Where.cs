using EQuery.Core;

namespace EQuery.Sql.SqlNode
{
    class Where : ISqlNode
    {
        private IExprNode expr;

        public Where(IExprNode expr)
        {
            this.expr = expr;
        }

        public void Render(SqlWriter writer)
        {                     
            writer.AppendLine(SqlStrings.Where);
            expr.Render(writer);
        }
    }
}
