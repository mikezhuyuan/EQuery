namespace EQuery.Sql.SqlNode
{
    class GreaterThanOrEqual : Relational
    {
        public override string Operator
        {
            get { return SqlStrings.GreaterThanOrEqual; }
        }
    }
}