namespace EQuery.Sql.SqlNode
{
    class GreaterThan : Relational
    {
        public override string Operator
        {
            get { return SqlStrings.GreaterThan; }
        }
    }
}