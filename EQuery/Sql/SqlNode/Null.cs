using EQuery.Core;

namespace EQuery.Sql.SqlNode
{
    class Null : IExprNode
    {
        public static readonly Null Instance = new Null();

        private Null()
        {
        }

        public void Render(SqlWriter writer)
        {
            writer.Append(SqlStrings.Null);
        }

        public Precedence Precedence
        {
            get { return Precedence.Hightest; }
        }
    }
}