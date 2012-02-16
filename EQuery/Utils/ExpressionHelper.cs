using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace EQuery.Utils
{
    static class ExpressionHelper
    {
        public static PropertyInfo GetPropertyInfo(LambdaExpression expr)
        {
            return (PropertyInfo)((MemberExpression)expr.Body).Member;
        }

        public static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }  

        public static LambdaExpression GetLambda(Expression e)
        {
            return StripQuotes(e) as LambdaExpression;
        }

        public static PropertyInfo[] GetPropertyPath(Expression expr)
        {
            var path = new Stack<PropertyInfo>();

            while (expr.NodeType == ExpressionType.MemberAccess)
            {
                var mexpr = (MemberExpression)expr;
                var propInfo = mexpr.Member as PropertyInfo;
                if (propInfo == null)
                    break;

                path.Push(propInfo);
                expr = mexpr.Expression;
            }

            return path.ToArray();
        }
    }
}
