using System;

namespace AutoFilter.Interfaces
{
    /// <summary>
    /// Represents a range of values to filter by.
    /// </summary>
    /// <typeparam name="T">The type of a single value in the range.</typeparam>
    public interface IRangeFilter<T>
        where T : struct, IComparable<T>
    {
        /// <summary>
        /// The minimal value to filter by.
        /// </summary>
        T? MinValue { get; set; }

        /// <summary>
        /// The maximal value to filter by.
        /// </summary>
        T? MaxValue { get; set; }

        /// <summary>
        /// An exact value to filter by.
        /// </summary>
        T? ExactValue { get; set; }
    }
}
