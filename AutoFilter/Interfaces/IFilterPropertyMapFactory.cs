using System;
using System.Linq.Expressions;

namespace AutoFilter.Interfaces
{
    /// <summary>
    /// Provides functionality to create a new <see cref="IFilterPropertyMap{TFilter, TEntity, TFilterProperty, TEntityProperty}"/>.
    /// </summary>
    public interface IFilterPropertyMapFactory<TFilter, TEntity>
        where TFilter : class
        where TEntity : class
    {
        /// <summary>
        /// Creates new <see cref="IFilterPropertyMap{TFilter, TEntity, TFilterProperty, TEntityProperty}"/>.
        /// </summary>
        /// <typeparam name="TFilterProperty">The type of the filter property.</typeparam>
        /// <typeparam name="TEntityProperty">The type of the entity property.</typeparam>
        /// <param name="filterProperty">Lambda that access the filter property.</param>
        /// <param name="entityProperty">Lambda that access the entity property.</param>
        /// <param name="predicate">The default predicate expression that will be used to compare between the filter and entity properties. Can be null.</param>
        /// <returns>A new instance of <see cref="IFilterPropertyMap{TFilter, TEntity, TFilterProperty, TEntityProperty}"/>.</returns>
        IFilterPropertyMap<TFilter, TEntity, TFilterProperty, TEntityProperty> CreateFilterPropertyMap<TFilterProperty, TEntityProperty>
            (Expression<Func<TFilter, TFilterProperty>> filterProperty,
             Expression<Func<TEntity, TEntityProperty>> entityProperty,
             Expression<Func<TFilterProperty, TEntityProperty, bool>> predicate);
        
    }
}
