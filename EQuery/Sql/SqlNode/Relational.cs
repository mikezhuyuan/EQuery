using System.Linq.Expressions;

namespace EQuery.Sql.SqlNode
{
    abstract class Relational : Binary //==, !=, >, <, <=, >=
    {
        public override Precedence Precedence
        {
            get { return Precedence.Medium; }
        }

        public static Relational CreateFrom(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                    return new Equal();
                case ExpressionType.NotEqual:
                    return new NotEqual();
                case ExpressionType.LessThan:
                    return new LessThan();
                case ExpressionType.LessThanOrEqual:
                    return new LessThanOrEqual();
                case ExpressionType.GreaterThan:
                    return new GreaterThan();
                case ExpressionType.GreaterThanOrEqual:
                    return new GreaterThanOrEqual();
            }

            return null;
        }
    }
}