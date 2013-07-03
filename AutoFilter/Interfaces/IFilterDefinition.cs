using System;
using System.Linq.Expressions;

namespace AutoFilter.Interfaces
{
    ///<summary>
    /// Allows custom mapping of filter object
    ///</summary>
    ///<typeparam name="TFilter">Filter type</typeparam>
    ///<typeparam name="TEntity">Entity type</typeparam>
    public interface IFilterDefinition<TFilter, TEntity>
        where TFilter : class
        where TEntity : class
    {
        /// <summary>
        /// Maps custom expression to filter property
        /// </summary>
        /// <typeparam name="TFilterProperty"></typeparam>
        /// <typeparam name="TEntityProperty"></typeparam>
        /// <param name="filterProperty">filter property to map</param>
        /// <param name="entityProperty">related entity property</param>
        /// <returns></returns>
        IFilterPropertyMap<TFilter, TEntity, TFilterProperty, TEntityProperty> Map<TFilterProperty, TEntityProperty>(
                        Expression<Func<TFilter, TFilterProperty>> filterProperty,
                        Expression<Func<TEntity, TEntityProperty>> entityProperty);

        /// <summary>
        /// Maps custom expression to filter property
        /// </summary>
        /// <typeparam name="TFilterProperty"></typeparam>
        /// <typeparam name="TEntityProperty"></typeparam>
        /// <param name="filterProperty">filter property to map</param>
        /// <param name="entityProperty">related entity property</param>
        /// <param name="predicateComparison">predicate comparison</param>
        /// <returns></returns>
        IFilterPropertyMap<TFilter, TEntity, TFilterProperty, TEntityProperty> Map<TFilterProperty, TEntityProperty>(
                        Expression<Func<TFilter, TFilterProperty>> filterProperty,
                        Expression<Func<TEntity, TEntityProperty>> entityProperty,
                        ComparisonType predicateComparison);

        /// <summary>
        /// Maps a range property from a filter object to a valve property in the destination type
        /// </summary>
        /// <typeparam name="TFilterProperty"></typeparam>
        /// <typeparam name="TEntityProperty"></typeparam>
        /// <param name="filterProperty">filter range property to map</param>
        /// <param name="entityProperty">related entity property</param>
        /// <returns></returns>
        [Obsolete("Use the Map method instead")]
        IFilterDefinition<TFilter, TEntity> MapRange<TFilterProperty, TEntityProperty>(
                        Expression<Func<TFilter, TFilterProperty>> filterProperty,
                        Expression<Func<TEntity, TEntityProperty>> entityProperty)
            where TFilterProperty : IRangeFilter<TEntityProperty>
            where TEntityProperty : struct, IComparable<TEntityProperty>;

        /// <summary>
        /// Ignore a filter property and exclude it from the filtering expression
        /// </summary>
        /// <typeparam name="TFilterProperty"></typeparam>
        /// <param name="filterProperty">The filter property to ignore</param>
        /// <returns></returns>
        IFilterDefinition<TFilter, TEntity> Ignore<TFilterProperty>(Expression<Func<TFilter, TFilterProperty>> filterProperty);
    }
}