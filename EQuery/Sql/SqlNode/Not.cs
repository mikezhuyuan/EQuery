namespace EQuery.Sql.SqlNode
{
    class Not : UnaryExpr
    {
        public override string Operator
        {
            get { return SqlStrings.Not; }
        }

        public override Precedence Precedence
        {
            get { return Precedence.Medium; }
        }
    }
}