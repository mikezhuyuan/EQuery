using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EQuery.Core;
using EQuery.Mapping;
using EQuery.Sql.SqlNode;
using EQuery.Utils;

namespace EQuery.Sql
{
    class WhereConverter
    {
        private static readonly MethodInfo Contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static readonly MethodInfo StartsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static readonly MethodInfo EndsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        private static readonly List<MethodInfo> StringMethods = new List<MethodInfo> { StartsWith, EndsWith, Contains, };

        private readonly Select _select;
        private readonly QueryContext _context;

        public WhereConverter(Select select, QueryContext context)
        {
            if (select == null)
                throw new ArgumentNullException("select");

            if (context == null)
                throw new ArgumentNullException("context");

            _select = select;
            _context = context;
        }

        public Where Visit(LambdaExpression expr)
        {
            return new Where(VisitBoolean(null, expr.Body));
        }

        private IExprNode VisitBoolean(IExprNode parent, Expression expr)
        {
            return VisitRelational(parent, expr) ?? VisitConditional(parent, expr) ?? VisitStringLike(parent, expr) ?? VisitMemberPath(parent, expr);
        }

        private IExprNode VisitConstant(IExprNode parent, Expression expr)
        {
            if (expr.NodeType != ExpressionType.Constant)
                return null;

            var constant = (ConstantExpression)expr;

            if (constant.Value == null)
                return Null.Instance;

            return new Parameter(new ConstantProvider(_context, constant));
        }

        private IExprNode VisitVariable(IExprNode parent, Expression expr)
        {
            if (expr.NodeType != ExpressionType.MemberAccess)
                return null;
            
            return new Parameter(new ConstantProvider(_context, (MemberExpression)expr));
        }

        private IExprNode VisitParameter(IExprNode parent, Expression expr)
        {
            if (expr.NodeType != ExpressionType.Parameter)
                return null;

            return new Parameter(new ParameterProvider(_context, (ParameterExpression)expr));
        }

        private IExprNode VisitStringLike(IExprNode parent, Expression expr)
        {
            if (expr.NodeType != ExpressionType.Call)
                return null;

            var method = (MethodCallExpression)expr;

            int index = StringMethods.IndexOf(method.Method) + 1;
            if (index <= 0)
                return null;

            var arg = method.Arguments[0];

            var result = new StringLike
            {
                Context = _context,
                PartialPrefix = (index & 1) == 1,
                PartialPostfix = (index & 2) == 2,
            };

            var property = VisitMemberPath(result, method.Object);
            var parameter = VisitConstant(result, arg) 
                         ?? VisitVariable(result, arg)
                         ?? VisitParameter(result, arg);

            result.PropertyAccess = property;
            result.Parameter = (Parameter)parameter;

            return result;
        }

        private IExprNode VisitConditional(IExprNode parent, Expression expr)
        {
            var type = expr.NodeType;
            if (type == ExpressionType.AndAlso)
            {
                var condition = (BinaryExpression)expr;
                var result = new And();
                result.Left = VisitBoolean(result, condition.Left);
                result.Right = VisitBoolean(result, condition.Right);

                return result;
            }

            if (type == ExpressionType.OrElse)
            {
                var condition = (BinaryExpression)expr;
                var result = new Or();
                result.Left = VisitBoolean(result, condition.Left);
                result.Right = VisitBoolean(result, condition.Right);

                return result;
            }

            if (type == ExpressionType.Not)
            {
                var condition = (UnaryExpression)expr;
                var result = new Not();
                result.Expr = VisitBoolean(result, condition.Operand);

                return result;
            }

            return null;
        }

        private IExprNode VisitRelational(IExprNode parent, Expression expr)
        {
            var condition = expr as BinaryExpression;
            if (condition == null)
                return null;

            var result = Relational.CreateFrom(expr.NodeType);
            if (result == null)
                return null;

            result.Left = VisitMemberPath(result, condition.Left);
            result.Right = VisitConstant(result, condition.Right) 
                        ?? VisitVariable(result, condition.Right) 
                        ?? VisitMemberPath(result, condition.Right)
                        ?? VisitParameter(result, condition.Right);

            return result;
        }

        private IExprNode VisitMemberPath(IExprNode parent, Expression expr)
        {
            if (expr.NodeType != ExpressionType.MemberAccess)
                return null;

            var path = ExpressionHelper.GetPropertyPath(expr);

            BracketedName alias;
            PropertyMap map;
            _select.VisitPath(_context, path, out alias, out map);

            return new PropertyAccess
            {
                Alias = alias,
                PropertyMap = map,
                Parent = parent
            };
        }
    }
}