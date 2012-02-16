namespace EQuery.Sql.SqlNode
{
    class Equal : Relational
    {
        public override string Operator
        {
            get
            {                
                if (Right is Null)
                    return SqlStrings.Is;                

                return SqlStrings.Equal;
            }
        }
    }
}