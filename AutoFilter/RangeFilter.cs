using System;
using System.Runtime.Serialization;
using AutoFilter.Interfaces;

namespace AutoFilter
{
    /// <summary>
    /// Represents a range of values to filter by.
    /// </summary>
    /// <typeparam name="T">The type of a single value in the range.</typeparam>
    [DataContract]
    public class RangeFilter<T> : IRangeFilter<T>
        where T : struct, IComparable<T>
    {
        #region Properties

        /// <summary>
        /// Minimal value in range
        /// </summary>
        [DataMember]
        public T? MinValue { get; set; }

        /// <summary>
        /// Maximum value in range
        /// </summary>
        [DataMember]
        public T? MaxValue { get; set; }

        /// <summary>
        /// Exact value
        /// </summary>
        [DataMember]
        public T? ExactValue { get; set; }

        #endregion

        #region Ctor
        /// <summary>
        /// Default C'tor
        /// </summary>
        public RangeFilter()
        {
        }

        /// <summary>
        /// Exact Value C'tor
        /// </summary>
        /// <param name="exactValue">value to compare against</param>
        public RangeFilter(T? exactValue)
        {
            ExactValue = exactValue;
        }

        /// <summary>
        /// range C'tor
        /// </summary>
        /// <param name="minValue">minimum value</param>
        /// <param name="maxValue">maximum  value</param>
        public RangeFilter(T? minValue, T? maxValue)
        {
            MaxValue = maxValue;
            MinValue = minValue;
        }

        #endregion
    }
}
