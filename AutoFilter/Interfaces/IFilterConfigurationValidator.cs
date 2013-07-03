using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoFilter.Interfaces
{
    public interface IFilterConfigurationValidator
    {
        /// <summary>
        /// Validate that all properties in all filters were mapped or ignored
        /// </summary>
        void AssertConfigurationIsValid();
    }
}
