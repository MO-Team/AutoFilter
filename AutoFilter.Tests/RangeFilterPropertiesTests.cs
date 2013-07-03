using System;
using System.Linq;
using AutoFilter;
using NUnit.Framework;
using System.Linq.Expressions;

namespace AutoFilter.Tests
{
    [TestFixture]
    public class RangeFilterPropertiesTests
    {
        [Test]
        public void RangeFilter_FilterHasRangePropWithSameNameAsTargetProp_AutomaticallyFilterByExactValue()
        {
            // Arrange
            var config = new FilterConfiguration();
            config.CreateFilter<FilterProductByDate, Product>();
            var exactDate = DateTime.Parse("01/01/2011 11:12:13.14"); 
            var filter = new FilterProductByDate {PurchaseDate = new DateRange(exactDate)};
            var products = new []
                               {
                                   new Product {Id = 1, PurchaseDate = exactDate.AddMinutes(5)},
                                   new Product {Id = 2, PurchaseDate = exactDate.AddMinutes(-5)},
                                   new Product {Id = 3, PurchaseDate = exactDate}
                               };

            var engine = new FilterEngine(config);

            // Act
            var query = engine.BuildExpression<FilterProductByDate, Product>(filter);
            var result = products.Where(query.Compile()).Single();

            // Assert
            Assert.That(result.Id, Is.EqualTo(3));
        }

        [Test]
        public void RangeFilter_FilterHasRangePropWithSameNameAsTargetProp_AutomaticallyFilterByMinAndMaxValues()
        {
            // Arrange
            var config = new FilterConfiguration();
            config.CreateFilter<FilterProductByDate, Product>();
            var exactDate = DateTime.Parse("01/01/2011 11:12:13.14");
            var filter = new FilterProductByDate
            {
                PurchaseDate = new DateRange(exactDate.AddMinutes(2), exactDate.AddMinutes(6))
            };

            var products = new[]
                               {
                                   new Product {Id = 1, PurchaseDate = exactDate.AddMinutes(5)},
                                   new Product {Id = 2, PurchaseDate = exactDate.AddMinutes(-5)},
                                   new Product {Id = 3, PurchaseDate = exactDate},
                                   new Product {Id = 4, PurchaseDate = exactDate.AddMinutes(3)},
                               };

            var engine = new FilterEngine(config);

            // Act
            var query = engine.BuildExpression<FilterProductByDate, Product>(filter);
            var result = products.Where(query.Compile()).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[1].Id, Is.EqualTo(4));
        }

        [Test]
        public void RangeFilter_FilterHasRangePropThatsManuallyMappedToTargetProp_DontFilter()
        {
            // Arrange
            var config = new FilterConfiguration();
            config.CreateFilter<FilterProductByDate, Product>().Map(f => f.SomeOtherDate, t => t.PurchaseDate);

            var exactDate = DateTime.Parse("01/01/2011 11:12:13.14"); 
            var filter = new FilterProductByDate { SomeOtherDate = new DateRange(exactDate) };
            var engine = new FilterEngine(config);

            var products = new[]
                               {
                                   new Product {Id = 1, PurchaseDate = exactDate.AddMinutes(5)},
                                   new Product {Id = 2, PurchaseDate = exactDate.AddMinutes(-5)},
                                   new Product {Id = 3, PurchaseDate = exactDate},
                                   new Product {Id = 4, PurchaseDate = exactDate.AddMinutes(3)},
                               };
            // Act
            var query = engine.BuildExpression<FilterProductByDate, Product>(filter);

            // Assert
            var result = products.Where(query.Compile()).ToList();
            Assert.That(result.Single().Id, Is.EqualTo(3));
        }

        [Test]
        public void RangeFilter_FilterHasRangePropThatsManuallyMappedToTargetPropWithPredicate_FilterByPredicate()
        {
            // Arrange
            var config = new FilterConfiguration();
            config.CreateFilter<FilterProductByDate, Product>()
                        .Map(f => f.SomeOtherDate, t => t.AnotherDate)
                        .UsePredicate((range, value) => range.MinValue.Equals(value));

            var exactDate = DateTime.Parse("01/01/2011 11:12:13.14"); 

            var filter = new FilterProductByDate { SomeOtherDate = new DateRange(exactDate, exactDate.AddMinutes(10)) };
            var products = new[]
                               {
                                   new Product {Id = 1, AnotherDate = exactDate.AddMinutes(5)},
                                   new Product {Id = 2, AnotherDate = exactDate},
                                   new Product {Id = 3, AnotherDate = exactDate.AddMinutes(-5)},
                               };

            var engine = new FilterEngine(config);

            // Act
            var query = engine.BuildExpression<FilterProductByDate, Product>(filter);
            var result = products.Where(query.Compile()).Single();

            // Assert
            Assert.That(result.Id, Is.EqualTo(2));
        }

        [Test]
        public void RangeFilter_FilterHasRangePropThatsRangeMappedToTargetProp_FilterByRange()
        {
            // Arrange
            var config = new FilterConfiguration();
            config.CreateFilter<FilterProductByDate, Product>()
                    .Map(f => f.SomeOtherDate, t => t.AnotherDate);

            var exactDate = DateTime.Parse("01/01/2011 11:12:13.14"); 

            var filter = new FilterProductByDate { SomeOtherDate = new DateRange(exactDate, exactDate.AddMinutes(10)) };
            var products = new[]
                               {
                                   new Product {Id = 1, AnotherDate = exactDate.AddMinutes(5)},
                                   new Product {Id = 2, AnotherDate = exactDate},
                                   new Product {Id = 3, AnotherDate = exactDate.AddMinutes(-5)},
                               };

            var engine = new FilterEngine(config);

            // Act
            var query = engine.BuildExpression<FilterProductByDate, Product>(filter);
            var result = products.Where(query.Compile()).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[1].Id, Is.EqualTo(2));
        }

        [Test]
        public void RangeFilter_FilterIsNullFilterByNull_FilterNullValues()
        {
            // Arrange
            var config = new FilterConfiguration();
            config.CreateFilter<FilterProductByDate, Product>()
                    .Map(f => f.SomeOtherDate, t => t.NullableDate).WhenNull().FilterByNull();


            var filter = new FilterProductByDate { SomeOtherDate = null };
            var products = new[]
                               {
                                   new Product {Id = 1, NullableDate = null},
                                   new Product {Id = 2, NullableDate = DateTime.Now}
                               };

            var engine = new FilterEngine(config);

            // Act
            var query = engine.BuildExpression<FilterProductByDate, Product>(filter);
            var result = products.Where(query.Compile()).ToList();

            // Assert
            Assert.That(result.Single().Id, Is.EqualTo(1));
        }

        [Test]
        public void RangeFilter_FilterToNullableTarget_FilterByRange()
        {
            // Arrange
            var config = new FilterConfiguration();
            config.CreateFilter<FilterProductByDate, Product>()
                    .Map(f => f.SomeOtherDate, t => t.NullableDate);

            var date = DateTime.Parse("01/01/2011 11:12:13.14");

            var filter = new FilterProductByDate { SomeOtherDate = new DateRange(date, date.AddMinutes(10)) };
            var products = new[]
                               {
                                   new Product {Id = 1, NullableDate = date.AddMinutes(5)},
                                   new Product {Id = 2, NullableDate = date},
                                   new Product {Id = 3, NullableDate = date.AddMinutes(-5)},
                               };

            var engine = new FilterEngine(config);

            // Act
            var query = engine.BuildExpression<FilterProductByDate, Product>(filter);
            var result = products.Where(query.Compile()).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[1].Id, Is.EqualTo(2));
        }

        [Test]
        public void RangeFilter_FilterToNull_EmptyResult()
        {
            // Arrange
            var config = new FilterConfiguration();
            config.CreateFilter<FilterProductByDate, Product>()
                    .Map(f => f.SomeOtherDate, t => t.NullableDate);

            var date = DateTime.Parse("01/01/2011 11:12:13.14");

            var filter = new FilterProductByDate { SomeOtherDate = new DateRange(date, date.AddMinutes(10)) };
            var products = new[]
                               {
                                   new Product {Id = 1, NullableDate = null},
                                   new Product {Id = 2, NullableDate = null},
                                   new Product {Id = 3, NullableDate = null},
                               };

            var engine = new FilterEngine(config);

            // Act
            var query = engine.BuildExpression<FilterProductByDate, Product>(filter);
            var result = products.Where(query.Compile()).ToList();

            // Assert
            Assert.That(result, Is.Empty);
        }

    }

    public class FilterProductByDate
    {
        public DateRange PurchaseDate { get; set; }
        public DateRange SomeOtherDate { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime AnotherDate { get; set; }
        public DateTime? NullableDate { get; set; }
    }
}