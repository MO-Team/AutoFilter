using System.Collections.Generic;
using System.Linq.Expressions;

namespace AutoFilter
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ExpressionUpdateExtensions
    {
        public static BinaryExpression Update(this BinaryExpression thisExpression, Expression left, LambdaExpression conversion, Expression right)
        {
            if (((left == thisExpression.Left) && (right == thisExpression.Right)) && (conversion == thisExpression.Conversion))
            {
                return thisExpression;
            }
            return Expression.MakeBinary(thisExpression.NodeType, left, right, thisExpression.IsLiftedToNull, thisExpression.Method, conversion);
        }

        public static ConditionalExpression Update(this ConditionalExpression thisExpression, Expression test, Expression ifTrue, Expression ifFalse)
        {
            if (((test == thisExpression.Test) && (ifTrue == thisExpression.IfTrue)) && (ifFalse == thisExpression.IfFalse))
            {
                return thisExpression;
            }
            return Expression.Condition(test, ifTrue, ifFalse);
        }

        public static InvocationExpression Update(this InvocationExpression thisExpression, Expression expression, IEnumerable<Expression> arguments)
        {
            if ((expression == thisExpression.Expression) && (arguments == thisExpression.Arguments))
            {
                return thisExpression;
            }
            return Expression.Invoke(expression, arguments);
        }

        public static MemberExpression Update(this MemberExpression thisExpression, Expression expression)
        {
            if (expression == thisExpression.Expression)
            {
                return thisExpression;
            }
            return Expression.MakeMemberAccess(expression, thisExpression.Member);
        }

        public static MethodCallExpression Update(this MethodCallExpression thisExpression, Expression @object, IEnumerable<Expression> arguments)
        {
            if ((@object == thisExpression.Object) && (arguments == thisExpression.Arguments))
            {
                return thisExpression;
            }
            return Expression.Call(@object, thisExpression.Method, arguments);
        }

        public static UnaryExpression Update(this UnaryExpression thisExpression, Expression operand)
        {
            if (operand == thisExpression.Operand)
            {
                return thisExpression;
            }
            return Expression.MakeUnary(thisExpression.NodeType, operand, thisExpression.Type, thisExpression.Method);
        }

        public static Expression<TDelegate> Update<TDelegate>(this Expression<TDelegate> thisExpression, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if ((body == thisExpression.Body) && (parameters == thisExpression.Parameters))
            {
                return thisExpression;
            }
            return Expression.Lambda<TDelegate>(body, parameters);
        }
    }
}
