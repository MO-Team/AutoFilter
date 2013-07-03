using System;
using System.Collections.Generic;
using AutoFilter;

namespace AutoFilter.Tests
{
    public class TestCatalogFilter
    {
        public TestCatalogFilter()
        {
            DataInsertionDate = new DateRange();
        }

        public DateRange DataInsertionDate { get; set; }

        public List<string> EditionName { get; set; }

        public string ProductName { get; set; }
    }

    public class DateRange:RangeFilter<DateTime>
    {
        public DateRange()
        {
        }

        public DateRange(DateTime? exactValue) : base(exactValue)
        {
        }

        public DateRange(DateTime? minValue, DateTime? maxValue) : base(minValue, maxValue)
        {
        }
    }
}
