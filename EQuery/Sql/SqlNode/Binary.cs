using EQuery.Core;

namespace EQuery.Sql.SqlNode
{
    abstract class Binary : IExprNode
    {
        public IExprNode Left;
        public IExprNode Right;
        
        public virtual void Render(SqlWriter writer)
        {            
            if(Precedence > Left.Precedence)
            {
                writer.Append("("); 
                writer.Append(Left);                
                writer.Append(")");
            }
            else
            {                
                writer.Append(Left);
            }
            
            writer.Append(Operator);

            if (Precedence > Right.Precedence)
            {
                writer.Append("(");
                writer.Append(Right);
                writer.Append(")");
            }
            else
            {
                writer.Append(Right);
            }            
        }

        public abstract string Operator { get; }
        public abstract Precedence Precedence { get; }
    }
}