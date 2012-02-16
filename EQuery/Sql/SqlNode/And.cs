namespace EQuery.Sql.SqlNode
{
    class And : Binary
    {
        public override string Operator
        {
            get { return SqlStrings.And; }
        }

        public override Precedence Precedence
        {
            get { return Precedence.Low; }
        }
    }
}