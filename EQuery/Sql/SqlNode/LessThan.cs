namespace EQuery.Sql.SqlNode
{
    class LessThan : Relational
    {
        public override string Operator
        {
            get { return SqlStrings.LessThan; }
        }
    }
}