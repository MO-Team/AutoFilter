using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoFilter
{
    /// <summary>
    /// Thrown when validation of a filter failed (one on more properties in a filter were not mapped)
    /// </summary>
    [Serializable]
    public class FilterPropertyMissingMappingException : Exception
    {
        private const string MessageFormat = @"Filter class '{0}' is missing a mapping for the following properties:{1}
All properties must be mapped to an entity property or ignored.";
        private static string PropertySeparator = Environment.NewLine + " ";


        /// <summary>
        /// Initialize new FilterPropertyMissingMappingException
        /// </summary>
        /// <param name="filterType">The filter class</param>
        /// <param name="unmappedProperties">A collection contains names of all unmapped properties</param>
        public FilterPropertyMissingMappingException(Type filterType, IEnumerable<string> unmappedProperties)
            : base(String.Format(MessageFormat, filterType.FullName, PropertySeparator + String.Join(PropertySeparator, unmappedProperties))) { }

        /// <summary>
        /// Initialize new FilterPropertyMissingMappingException
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected FilterPropertyMissingMappingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
