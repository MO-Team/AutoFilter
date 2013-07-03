using System;
using System.Linq.Expressions;
using System.Reflection;
using AutoFilter.Interfaces;


namespace AutoFilter
{
    /// <summary>
    /// Provides functionality to create an expression that can logically compare two different types.
    /// </summary>
    public abstract class BasePredicateProvider : IPredicateProvider
    {
        #region Public Methods

        /// <summary>
        /// Determine if a predicate expression can be created for the specified arguments types.
        /// </summary>
        /// <typeparam name="T1">First predicate argument type.</typeparam>
        /// <typeparam name="T2">Second predicate argument type.</typeparam>
        /// <param name="predicateComparison">The comparison to use between the arguments.</param>
        /// <returns>True if  predicate expression can be created, otherwise false.</returns>
        public bool CanBuildPredicateExpression<T1, T2>(ComparisonType predicateComparison)
        {
            return CanBuildPredicateExpression(typeof(T1), typeof(T2), predicateComparison);
        }

        /// <summary>
        /// Determine if a predicate expression can be created for the specified arguments types.
        /// </summary>
        /// <param name="t1">First predicate argument type.</param>
        /// <param name="t2">Second predicate argument type.</param>
        /// <param name="predicateComparison">The comparison to use between the arguments.</param>
        /// <returns>True if  predicate expression can be created, otherwise false.</returns>
        public abstract bool CanBuildPredicateExpression(Type t1, Type t2, ComparisonType predicateComparison);
        
        /// <summary>
        /// Creates a Lambda Expression that will return true if the two arguments are logically equal or false otherwise.
        /// </summary>
        /// <typeparam name="T1">First predicate argument type.</typeparam>
        /// <typeparam name="T2">Second predicate argument type.</typeparam>
        /// <param name="predicateComparison">The comparison to use between the arguments.</param>
        /// <returns>The predicate function that compare the two types.</returns>
        public abstract Expression<Func<T1, T2, bool>> BuildPredicateExpression<T1, T2>(ComparisonType predicateComparison);

        /// <summary>
        /// Creates a Lambda Expression that will return true if the two arguments are logically equal or false otherwise.
        /// </summary>
        /// <param name="t1">First predicate argument type.</param>
        /// <param name="t2">Second predicate argument type.</param>
        /// <param name="predicateComparison">The comparison to use between the arguments.</param>
        /// <returns>The predicate function that compare the two types.</returns>
        public LambdaExpression BuildPredicateExpression(Type t1, Type t2, ComparisonType predicateComparison)
        {
            Ensure.ArgumentNotNull(() => t1);
            Ensure.ArgumentNotNull(() => t2);

            var buildPredicateMethod =
                TypeUtils.GetMethodInfo((IPredicateProvider p) => p.BuildPredicateExpression<object, object>(ComparisonType.Equal))
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(t1, t2);

            try
            {
                return buildPredicateMethod.Invoke(this, new object[] { predicateComparison }) as LambdaExpression;
            }
            catch (TargetInvocationException exc)
            {
                throw exc.InnerException;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// translates ComparisonType to comparison expression 
        /// </summary>
        /// <param name="predicateComparison">ComparisonType enumeration</param>
        /// <returns>comparison expression</returns>
        protected internal ExpressionType GetBinaryExpressionType(ComparisonType predicateComparison)
        {
            switch (predicateComparison)
            {
                case ComparisonType.Equal:
                    return ExpressionType.Equal;
                case ComparisonType.GreaterThan:
                    return ExpressionType.GreaterThan;
                case ComparisonType.GreaterThanOrEqual:
                    return ExpressionType.GreaterThanOrEqual;
                case ComparisonType.LessThan:
                    return ExpressionType.LessThan;
                case ComparisonType.LessThanOrEqual:
                    return ExpressionType.LessThanOrEqual;

                default:
                    throw new ArgumentException(@"The predicateComparison value is not valid", "predicateComparison");
            }
        }

        #endregion
    }
}
