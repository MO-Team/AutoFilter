namespace AutoFilter.Interfaces
{
    /// <summary>
    /// Validate that all properties in all filters were mapped or ignored
    /// </summary>
    public interface IFilterConfigurationValidator
    {
        /// <summary>
        /// Validate that all properties in all filters were mapped or ignored
        /// </summary>
        void AssertConfigurationIsValid();
    }
}
