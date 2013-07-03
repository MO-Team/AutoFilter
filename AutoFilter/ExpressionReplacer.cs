using System.Collections.Generic;
using System.Linq.Expressions;

namespace AutoFilter
{
    /// <summary>
    /// Contains an extension methods for replacement of an expressions in an <see cref="Expression"/> Tree.
    /// </summary>
    public static class ExpressionReplacer
    {
        #region Public Methods

        /// <summary>
        /// Replace an expression in all child expression of a specified expression.
        /// </summary>
        /// <typeparam name="T">The expression type.</typeparam>
        /// <param name="expression">The expression to search in and its all child expressions.</param>
        /// <param name="expressionToReplace">The expression that will be replaced.</param>
        /// <param name="newExpression">The new expression to replace the expression expressionToReplace.</param>
        /// <returns>The updated expression after all child expression checked and replaced if necessary.</returns>
        public static T ReplaceExpression<T>(this T expression, Expression expressionToReplace, Expression newExpression)
            where T : Expression
        {
            Ensure.ArgumentNotNull(() => expressionToReplace);
            Ensure.ArgumentNotNull(() => newExpression);

            if (expression == null)
                return null;

            if (expression == expressionToReplace)
                return newExpression is T ? newExpression as T : expression;


            if (expression is BinaryExpression)
                return ReplaceExpressionInBinaryExpression(expression as BinaryExpression, expressionToReplace, newExpression) as T;

            if (expression is ConditionalExpression)
                return ReplaceExpressionInConditionalExpression(expression as ConditionalExpression, expressionToReplace, newExpression) as T;

            if (expression is InvocationExpression)
                return ReplaceExpressionInInvocationExpression(expression as InvocationExpression, expressionToReplace, newExpression) as T;

            if (expression is LambdaExpression)
                return ReplaceExpressionInLambdaExpression(expression as LambdaExpression, expressionToReplace, newExpression) as T;

            if (expression is MemberExpression)
                return ReplaceExpressionInMemberExpression(expression as MemberExpression, expressionToReplace, newExpression) as T;

            if (expression is MethodCallExpression)
                return ReplaceExpressionInMethodCallExpression(expression as MethodCallExpression, expressionToReplace, newExpression) as T;

            if (expression is UnaryExpression)
                return ReplaceExpressionInUnaryExpression(expression as UnaryExpression, expressionToReplace, newExpression) as T;

            return expression;
        }

        /// <summary>
        /// Replace an expression in a collection of expressions.
        /// </summary>
        /// <typeparam name="T">The expression type.</typeparam>
        /// <param name="expressionList">The collection of expressions to be searched.</param>
        /// <param name="expressionToReplace">The expression that will be replaced.</param>
        /// <param name="newExpression">The new expression to replace the expression expressionToReplace.</param>
        /// <returns>The updated expression after all child expression checked and replaced if necessary.</returns>
        public static ICollection<T> ReplaceExpressionInCollection<T>(this ICollection<T> expressionList, Expression expressionToReplace, Expression newExpression)
            where T : Expression
        {
            Ensure.ArgumentNotNull(() => expressionToReplace);
            Ensure.ArgumentNotNull(() => newExpression);

            if (expressionList == null)
                return null;

            var newList = new List<T>(expressionList.Count);
            foreach (var exp in expressionList)
            {
                newList.Add(ReplaceExpression(exp, expressionToReplace, newExpression));
            }
            return newList;
        }

        #endregion

        #region Private Methods

        private static BinaryExpression ReplaceExpressionInBinaryExpression(BinaryExpression binary, Expression expressionToReplace, Expression newExpression)
        {
            return binary.Update(ReplaceExpression(binary.Left, expressionToReplace, newExpression),
                                 ReplaceExpression(binary.Conversion, expressionToReplace, newExpression),
                                 ReplaceExpression(binary.Right, expressionToReplace, newExpression));
        }

        private static ConditionalExpression ReplaceExpressionInConditionalExpression(ConditionalExpression conditionalExpression, Expression expressionToReplace, Expression newExpression)
        {
            return conditionalExpression.Update(ReplaceExpression(conditionalExpression.Test, expressionToReplace, newExpression),
                                                ReplaceExpression(conditionalExpression.IfTrue, expressionToReplace, newExpression),
                                                ReplaceExpression(conditionalExpression.IfFalse, expressionToReplace, newExpression));
        }

        private static InvocationExpression ReplaceExpressionInInvocationExpression(InvocationExpression invocationExpression, Expression expressionToReplace, Expression newExpression)
        {
            return invocationExpression.Update(ReplaceExpression(invocationExpression.Expression, expressionToReplace, newExpression),
                                               ReplaceExpressionInCollection(invocationExpression.Arguments, expressionToReplace, newExpression));
        }

        private static LambdaExpression ReplaceExpressionInLambdaExpression(LambdaExpression lambda, Expression expressionToReplace, Expression newExpression)
        {
            var parameters = ReplaceExpressionInCollection(lambda.Parameters, expressionToReplace, newExpression);
            return Expression.Lambda(lambda.Type,
                                     ReplaceExpression(lambda.Body, expressionToReplace, newExpression),
                                     parameters);
        }

        private static MemberExpression ReplaceExpressionInMemberExpression(MemberExpression memberExpression, Expression expressionToReplace, Expression newExpression)
        {
            return memberExpression.Update(ReplaceExpression(memberExpression.Expression, expressionToReplace, newExpression));
        }

        private static MethodCallExpression ReplaceExpressionInMethodCallExpression(MethodCallExpression methodCallExpression, Expression expressionToReplace, Expression newExpression)
        {
            return methodCallExpression.Update(ReplaceExpression(methodCallExpression.Object, expressionToReplace, newExpression),
                                               ReplaceExpressionInCollection(methodCallExpression.Arguments, expressionToReplace, newExpression));
        }

        private static UnaryExpression ReplaceExpressionInUnaryExpression(UnaryExpression unary, Expression expressionToReplace, Expression newExpression)
        {
            return unary.Update(ReplaceExpression(unary.Operand, expressionToReplace, newExpression));
        }

        #endregion
    }
}
