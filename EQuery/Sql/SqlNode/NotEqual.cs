namespace EQuery.Sql.SqlNode
{
    class NotEqual : Relational
    {       
        public override string Operator
        {
            get
            {
                if (Right is Null)
                    return SqlStrings.Is + " " + SqlStrings.Not;

                return SqlStrings.NotEqual;
            }
        }
    }
}