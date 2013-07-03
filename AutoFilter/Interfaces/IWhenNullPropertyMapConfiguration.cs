namespace AutoFilter.Interfaces
{
    /// <summary>
    /// Defines the behavior when filter property has a null value 
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface IWhenNullPropertyMapConfiguration<TFilter, TEntity>
        where TFilter : class
        where TEntity : class
    {
        /// <summary>
        /// Don't filter by the property when it's null (Default)
        /// </summary>
        /// <returns></returns>
        IFilterDefinition<TFilter, TEntity> IgnoreProperty();

        /// <summary>
        /// Filter by the property when it's null instead of ignoring this property
        /// </summary>
        IFilterDefinition<TFilter, TEntity> FilterByNull();
    }
}
