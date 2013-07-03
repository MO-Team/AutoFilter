namespace AutoFilter.Interfaces
{
    ///<summary>
    /// Apply all registered filters.
    ///</summary>
    public interface IFilterApplier
    {
        ///<summary>
        /// Apply all assigned filters
        ///</summary>
        IFilterConfigurationValidator Apply();
    }
}