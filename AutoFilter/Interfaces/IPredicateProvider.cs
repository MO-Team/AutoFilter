using System;
using System.Linq.Expressions;

namespace AutoFilter.Interfaces
{
    /// <summary>
    /// Provides functionality to create an expression that can logically compare two different types.
    /// </summary>
    public interface IPredicateProvider
    {
        /// <summary>
        /// Determine if a predicate expression can be created for the specified arguments types.
        /// </summary>
        /// <typeparam name="T1">First predicate argument type.</typeparam>
        /// <typeparam name="T2">Second predicate argument type.</typeparam>
        /// <param name="predicateComparison">The comparison to use between the arguments.</param>
        /// <returns>True if  predicate expression can be created, otherwise false.</returns>
        bool CanBuildPredicateExpression<T1, T2>(ComparisonType predicateComparison);

        /// <summary>
        /// Determine if a predicate expression can be created for the specified arguments types.
        /// </summary>
        /// <param name="t1">First predicate argument type.</param>
        /// <param name="t2">Second predicate argument type.</param>
        /// <param name="predicateComparison">The comparison to use between the arguments.</param>
        /// <returns>True if  predicate expression can be created, otherwise false.</returns>
        bool CanBuildPredicateExpression(Type t1, Type t2, ComparisonType predicateComparison);

        /// <summary>
        /// Creates a Lambda Expression that will return true if the two arguments are logically equal or false otherwise.
        /// </summary>
        /// <typeparam name="T1">First predicate argument type.</typeparam>
        /// <typeparam name="T2">Second predicate argument type.</typeparam>
        /// <param name="predicateComparison">The comparison to use between the arguments.</param>
        /// <returns>The predicate function that compare the two types.</returns>
        Expression<Func<T1, T2, bool>> BuildPredicateExpression<T1, T2>(ComparisonType predicateComparison);

        /// <summary>
        /// Creates a Lambda Expression that will return true if the two arguments are logically equal or false otherwise.
        /// </summary>
        /// <param name="t1">First predicate argument type.</param>
        /// <param name="t2">Second predicate argument type.</param>
        /// <param name="predicateComparison">The comparison to use between the arguments.</param>
        /// <returns>The predicate function that compare the two types.</returns>
        LambdaExpression BuildPredicateExpression(Type t1, Type t2, ComparisonType predicateComparison);
    }
}
