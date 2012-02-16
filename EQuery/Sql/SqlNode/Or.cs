namespace EQuery.Sql.SqlNode
{
    class Or : Binary
    {
        public override string Operator
        {
            get { return SqlStrings.Or; }
        }

        public override Precedence Precedence
        {
            get { return Precedence.Lowest; }
        }
    }
}