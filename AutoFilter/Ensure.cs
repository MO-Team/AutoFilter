using System;
using System.Linq.Expressions;

namespace AutoFilter
{
    /// <summary>
    /// Validates arguments to ensure their state is correct.
    /// </summary>
    public static class Ensure
    {
        #region Consts

        private const string ExpressionNullError =
            "Can not check if argument is null, if its expression is null";

        #endregion

        #region Methods

        /// <summary>
        /// Ensures that a reference argument is not null. If the value is null then an ArgumentNullException is thrown (with the spcified name).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the argument in the expression is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the expression is null.</exception>
        /// <typeparam name="T">The type of the checked argument. The type must be a class.</typeparam>
        /// <param name="memberExpression">The expression of the argument to ensure.</param>
        public static void ArgumentNotNull<T>(Expression<Func<T>> memberExpression) where T : class
        {
            EnsureExpressionValueIsNotNull(memberExpression);
        }

        /// <summary>
        /// Ensures that a nullable argument is not null. If the value is null then an ArgumentNullException is thrown (with the spcified name).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the argument in the expression is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the expression is null.</exception>
        ///  <typeparam name="T">The type of the checked argument. The type must be a nullable struct.</typeparam>
        /// <param name="memberExpression">The expression of the argument to ensure.</param>
        public static void ArgumentNotNull<T>(Expression<Func<T?>> memberExpression) where T : struct
        {
            EnsureExpressionValueIsNotNull(memberExpression);
        }

        /// <summary>
        /// Ensures that a string argument is not null or empty. If it is, then an ArgumentNullException is thrown (with the spcified name).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the string in the expression is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the expression is null.</exception>
        /// <param name="memberExpression">The expression of the string argument to ensure.</param>
        public static void ArgumentNotNull(Expression<Func<string>> memberExpression)
        {
            ValidateExpressionNotNull(memberExpression);
            var value = memberExpression.Compile().Invoke();
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(GetArgumentName(memberExpression));
            }
        }

        #endregion

        #region Private methods

        private static void EnsureExpressionValueIsNotNull<T>(Expression<Func<T>> memberExpression)
        {
            ValidateExpressionNotNull(memberExpression);
            object value = memberExpression.Compile().Invoke();
            if (value == null)
            {
                throw new ArgumentNullException(GetArgumentName(memberExpression));
            }
        }

        private static void ValidateExpressionNotNull<T>(Expression<Func<T>> memberExpression)
        {
            if (memberExpression == null)
            {
                throw new InvalidOperationException(ExpressionNullError);
            }
        }

        private static string GetArgumentName<T>(Expression<Func<T>> memberExpression)
        {
            return ((MemberExpression)memberExpression.Body).Member.Name;
        }

        #endregion
    }
}
