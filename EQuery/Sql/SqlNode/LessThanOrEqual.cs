namespace EQuery.Sql.SqlNode
{
    class LessThanOrEqual : Relational
    {
        public override string Operator
        {
            get { return SqlStrings.LessThanOrEqual; }
        }
    }
}