using System;
using System.Linq.Expressions;

namespace EQuery.Core
{    
    interface IValueProvider
    {
        object Value { get; set; }
        string ParamName { get; }
    }

    class ConstantProvider : IValueProvider
    {
        public ConstantProvider(QueryContext context, ConstantExpression expr)
        {
            Value = expr.Value;
            
            ParamName = context.NextParam(this);
        }
        
        public ConstantProvider(QueryContext context, MemberExpression expr)
        {
            var constant = (ConstantExpression)expr.Expression;
            Value = constant.Type.GetField(expr.Member.Name).GetValue(constant.Value);

            ParamName = context.NextParam(this);
        }

        public ConstantProvider(QueryContext context, object value)
        {
            Value = value;

            ParamName = context.NextParam(this);
        }

        public object Value { get; set; }
        public string ParamName { get; private set; }
    }

    class ParameterProvider : IValueProvider
    {
        public ParameterProvider(QueryContext context, ParameterExpression expr)
        {
            ExpressionName = expr.Name;
            ParamName = context.NextParam(this);
        }

        public object Value { get; set; }
        public string ExpressionName { get; private set; }
        public string ParamName { get; private set; }

        public int Ordinal;        
    }
}
