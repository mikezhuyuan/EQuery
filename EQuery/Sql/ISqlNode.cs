using EQuery.Core;

namespace EQuery.Sql
{
    interface ISqlNode
    {
        void Render(SqlWriter writer);
    }
}