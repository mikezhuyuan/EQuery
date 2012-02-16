using EQuery.Sql.SqlNode;

namespace EQuery.Sql
{
    interface IExprNode : ISqlNode
    {
        Precedence Precedence { get; }
    }
}