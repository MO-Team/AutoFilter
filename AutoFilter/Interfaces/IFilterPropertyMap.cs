using System;
using System.Linq.Expressions;

namespace AutoFilter.Interfaces
{
    /// <summary>
    /// Represents a mapping between a filter property and an entity property.
    /// </summary>
    /// <typeparam name="TFilter">The filter type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TFilterProperty">The mapped filter property type.</typeparam>
    /// <typeparam name="TEntityProperty">The mapped entity property type.</typeparam>
    

    // DOESN'T THIS INTERFACE METHODS SHOULD GO TO ANOTHER INTERFACE? 
    //          (something like IPropertyMapConfiguration\IPropertyMapOptions) 
    //      - so the user will not see the methods in IFilterConditionExpressionBuilder
    public interface IFilterPropertyMap<TFilter, TEntity, TFilterProperty, TEntityProperty> : IFilterConditionExpressionBuilder<TFilter, TEntity>, IFilterDefinition<TFilter, TEntity>
        where TFilter : class
        where TEntity : class
    {
        /// <summary>
        /// Replace the predicate expression that used to determine the mapped entity property filter expression.
        /// </summary>
        /// <param name="predicate">The new predicate in Lambda or an Expression Tree.</param>
        /// <returns>An instance of <see cref="IFilterPropertyMap{TFilter, TEntity, TFilterProperty, TEntityProperty}"/> witch uses the new predicate expression.</returns>
        /// <remarks>Should be used as a (Fluent) Method Chain.</remarks>
        IFilterDefinition<TFilter, TEntity> UsePredicate(Expression<Func<TFilterProperty, TEntityProperty, bool>> predicate);

        /// <summary>
        /// Defines the behavior when filter property has a null value
        /// </summary>
        /// <returns></returns>
        IWhenNullPropertyMapConfiguration<TFilter, TEntity> WhenNull();

        /// <summary>
        /// Ignore the mapped filter property and exclude it from the filtering expression.
        /// </summary>
        IFilterDefinition<TFilter, TEntity> Ignore();
    }
}
