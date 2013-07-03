namespace AutoFilter.Interfaces
{
    /// <summary>
    /// Implementing ICreateFilter interface allows CreateFilter to load your filters automatically.
    /// </summary>
    public interface ICreateFilter
    {
        /// <summary>
        /// Map a filter type to an entity type
        /// </summary>
        /// <param name="filterConfiguration">Filter Configuration</param>
        void CreateFilter(IFilterConfiguration filterConfiguration);
    }
}