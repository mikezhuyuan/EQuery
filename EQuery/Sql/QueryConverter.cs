using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using EQuery.Core;
using EQuery.Mapping;
using EQuery.Sql.SqlNode;
using EQuery.Utils;

namespace EQuery.Sql
{
    class QueryConverter
    {
        private readonly QueryContext _context;        

        public QueryConverter(QueryContext context)
        {            
            _context = context;
        }

        public ISqlNode Visit(Expression expr)
        {
            return Visit(null, expr);
        }

        private ISqlNode Visit(ISqlNode parent, Expression expr)
        {
            if (expr.NodeType == ExpressionType.Constant && expr.Type.IsGenericType(typeof(Queryable<>)))
                return VisitRoot(parent, expr);

            if(expr.NodeType == ExpressionType.Call)
            {
                var call = (MethodCallExpression) expr;
                switch (call.Method.Name)
                {
                    case "Where":                       
                        return VisitWhere(Visit(parent, call.Arguments[0]), call);
                    case "OrderBy":
                        return VisitOrderBy(Visit(parent, call.Arguments[0]), call);
                    case "ThenBy":
                        return VisitThenBy(Visit(parent, call.Arguments[0]), call);
                    case "OrderByDescending":
                        return VisitOrderByDescending(Visit(parent, call.Arguments[0]), call);
                    case "ThenByDescending":
                        return VisitThenByDescending(Visit(parent, call.Arguments[0]), call);
                    case "Take":
                        return VisitTake(Visit(parent, call.Arguments[0]), call);
                    case "Skip":
                        return VisitSkip(Visit(parent, call.Arguments[0]), call);
                    case "First":
                    case "Single":
                    case "FirstOrDefault":                    
                    case "SingleOrDefault":
                        return VisitFirst(Visit(parent, call.Arguments[0]), call);                        
                }                
            }

            throw new NotSupportedException("Not supported expression: " + expr);
        }

        private ISqlNode VisitWhere(ISqlNode parent, MethodCallExpression expr)
        {
            var select = (Select)parent;
            var whereConverter = new WhereConverter(select, _context);
            var whereExpr = whereConverter.Visit(ExpressionHelper.GetLambda(expr.Arguments[1]));

            select.Where = whereExpr;

            return select;
        }

        private ISqlNode VisitSkip(ISqlNode parent, MethodCallExpression expr)
        {
            var select = (Select)parent;
            var arg = expr.Arguments[1];

            if (arg.NodeType == ExpressionType.Constant)
            {
                return new SelectPaging(_context.NextAlias(), select, new ConstantProvider(_context, (ConstantExpression)arg));
            }
            
            if (arg.NodeType == ExpressionType.Parameter)
            {
                return new SelectPaging(_context.NextAlias(), select, new ParameterProvider(_context, (ParameterExpression)arg));
            }

            throw new NotSupportedException();
        }

        private ISqlNode VisitTake(ISqlNode parent, MethodCallExpression expr)
        {
            var select = (ISelect)parent;
            var arg = expr.Arguments[1];

            if (arg.NodeType == ExpressionType.Parameter)
            {
                select.Top = new Top(new ParameterProvider(_context, (ParameterExpression)arg));
            }
            else if (arg.NodeType == ExpressionType.Constant)
            {
                select.Top = new Top(new ConstantProvider(_context, (ConstantExpression)arg));
            }
            else
            {
                throw new NotSupportedException();
            }

            return select;
        }

        private ISqlNode VisitFirst(ISqlNode parent, MethodCallExpression expr)
        {
            var select = (ISelect)parent;
            select.Top = new Top(new ConstantProvider(_context, 1));

            return select;
        }

        private ISqlNode VisitThenBy(ISqlNode parent, MethodCallExpression expr)
        {
            return VisitOrderBy(parent, expr, true);            
        }

        private ISqlNode VisitOrderByDescending(ISqlNode parent, MethodCallExpression expr)
        {
            return VisitOrderBy(parent, expr, false);
        }

        private ISqlNode VisitThenByDescending(ISqlNode parent, MethodCallExpression expr)
        {
            return VisitOrderBy(parent, expr, false);
        }

        private ISqlNode VisitOrderBy(ISqlNode parent, MethodCallExpression expr)
        {
            return VisitOrderBy(parent, expr, true);
        }

        private ISqlNode VisitOrderBy(ISqlNode parent, MethodCallExpression expr, bool ascending)
        {
            var select = (Select) parent;

            BracketedName alias;
            PropertyMap map;

            PropertyInfo[] path = ExpressionHelper.GetPropertyPath(ExpressionHelper.GetLambda(expr.Arguments[1]).Body);
            select.VisitPath(_context, path, out alias, out map);

            var orderBy = select.OrderBy ?? (select.OrderBy = new OrderBy());
            orderBy.AddItem(alias, map, ascending);            

            return select;
        }

        private ISqlNode VisitRoot(ISqlNode parent, Expression expr)
        {
            var select = new Select(_context);

            return select;
        }
    }
}
