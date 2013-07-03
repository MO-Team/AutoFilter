using System;
using System.Linq;
using AutoFilter.Interfaces;
using NUnit.Framework;

namespace AutoFilter.Tests
{
    [TestFixture]
    public class FluentConfigurationsSyntaxTests
    {
        private IFilterConfiguration _config;
        private IFilterEngine _engine;
        private MyProduct[] _products;
        private DateTime _testDate = DateTime.Parse("01/01/2011 11:12:13.14");

        [SetUp]
        public void Setup()
        {
            _config = new FilterConfiguration();

            // Define all sorts of mappings fluently:
            _config.CreateFilter<MyProductFilter, MyProduct>()
                // Map to ignore 
                .Map(f => f.IgnoreMe, s => s.IgnoreMe).Ignore()
                // Map using predicate
                .Map(f => f.ComapreWithPredicate, s => s.ComapreWithPredicate).UsePredicate(
                    (sFilter, sEntity) => sEntity.StartsWith(sFilter))
                // Map using comparison type
                .Map(f => f.ComapreWithComparisonType, s => s.ComapreWithComparisonType, ComparisonType.LessThan)
                // Map using range
                .Map(f => f.PurchaseDateRange, s => s.PurchaseDate)
                // Map another ignore, to make sure all the commands are fluent in every order
                .Map(f => f.IgnoreMeToo, s => s.IgnoreMeToo).Ignore()
                //Map with filter when null
                .Map(f => f.FilterMeAsNull, s => s.FilterMeAsNull).WhenNull().FilterByNull();

            // Validate that all filters configuration are valid
            _config.AssertConfigurationIsValid();

            _engine = new FilterEngine(_config);

            _products = new[]
                            {
                                new MyProduct
                                    {
                                        Id = 1,
                                        ComapreWithComparisonType = 10,
                                        ComapreWithPredicate = "matan",
                                        IgnoreMe = 5,
                                        PurchaseDate = _testDate.AddMinutes(1)
                                    },
                                new MyProduct
                                    {
                                        Id = 2,
                                        ComapreWithComparisonType = 20,
                                        ComapreWithPredicate = "mata",
                                        IgnoreMe = 5,
                                        PurchaseDate = _testDate.AddMinutes(2),
                                        FilterMeAsNull = "Not Null"
                                    },
                                new MyProduct
                                    {
                                        Id = 3,
                                        ComapreWithComparisonType = 30,
                                        ComapreWithPredicate = "mat",
                                        IgnoreMe = 5,
                                        PurchaseDate = _testDate.AddMinutes(3),
                                        FilterMeAsNull = "Not Null"
                                    },
                                new MyProduct
                                    {
                                        Id = 4,
                                        ComapreWithComparisonType = 40,
                                        ComapreWithPredicate = "ma",
                                        IgnoreMe = 5,
                                        PurchaseDate = _testDate.AddMinutes(4)
                                    },
                                new MyProduct
                                    {
                                        Id = 5,
                                        ComapreWithComparisonType = 50,
                                        ComapreWithPredicate = "m",
                                        IgnoreMe = 5,
                                        PurchaseDate = _testDate.AddMinutes(5)
                                    },
                            };
        }

        [Test]
        public void Map_MapMultipleMembersInTheSameStatment_IgnoreIsApplied()
        {
            // Arrange
            var filter = new MyProductFilter { IgnoreMe = 1, IgnoreMeToo = 1, FilterMeAsNull = "Not Null" };

            // Act
            var query = _engine.BuildExpression<MyProductFilter, MyProduct>(filter);

            // Assert
            var result = _products.Where(query.Compile()).ToList();
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void Map_MapMultipleMembersInTheSameStatment_MapWithPredicateIsApplied()
        {
            // Arrange
            var filter = new MyProductFilter { ComapreWithPredicate = "mata", FilterMeAsNull="Not Null" };

            // Act
            var query = _engine.BuildExpression<MyProductFilter, MyProduct>(filter);
            var result = _products.Where(query.Compile()).ToList();
            
            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(2));
        }

        [Test]
        public void Map_MapMultipleMembersInTheSameStatment_MapWithComparisonTypeIsApplied()
        {
            // Arrange
            var filter = new MyProductFilter { ComapreWithComparisonType = 40 };

            // Act
            var query = _engine.BuildExpression<MyProductFilter, MyProduct>(filter);
            var result = _products.Where(query.Compile());

            // Assert
            Assert.That(result.Single().Id, Is.EqualTo(5));
        }

        [Test]
        public void Map_MapMultipleMembersInTheSameStatment_MapRangeIsApplied()
        {
            // Arrange
            var filter = new MyProductFilter { PurchaseDateRange = new DateRange(_testDate.AddMinutes(3.5), _testDate.AddMinutes(4.5)) };

            // Act
            var query = _engine.BuildExpression<MyProductFilter, MyProduct>(filter);
            var result = _products.Where(query.Compile());

            // Assert
            Assert.That(result.Single().Id, Is.EqualTo(4));
        }

        [Test]
        public void Map_MapMultipleMembersInTheSameStatment_EverythingIsApplied()
        {
            // Arrange
            var filter = new MyProductFilter
                             {
                                 ComapreWithComparisonType = 25,
                                 ComapreWithPredicate = "mat",
                                 IgnoreMe = 1,
                                 IgnoreMeToo = 1,
                                 PurchaseDateRange = new DateRange(_testDate.AddMinutes(3), _testDate.AddMinutes(5)),
                                 FilterMeAsNull = "Not Null"

                             };

            // Act
            var query = _engine.BuildExpression<MyProductFilter, MyProduct>(filter);
            var result = _products.Where(query.Compile());

            // Assert
            Assert.That(result.Single().Id, Is.EqualTo(3));
        }

        [Test]
        public void Map_MapNullWithFilterNull_ApplyFilter()
        {
            // Arrange
            var filter = new MyProductFilter
            {
            };

            // Act
            var query = _engine.BuildExpression<MyProductFilter, MyProduct>(filter);
            var result = _products.Where(query.Compile());

            // Assert
            Assert.That(result.Count(), Is.EqualTo(3));
        }

    }

    public class MyProduct
    {
        public int Id { get; set; }
        public int IgnoreMe { get; set; }
        public int IgnoreMeToo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string ComapreWithPredicate { get; set; }
        public int ComapreWithComparisonType { get; set; }
        public string FilterMeAsNull { get; set; }
    }

    public class MyProductFilter
    {
        public int? IgnoreMe { get; set; }
        public int? IgnoreMeToo { get; set; }
        public DateRange PurchaseDateRange { get; set; }
        public string ComapreWithPredicate { get; set; }
        public int? ComapreWithComparisonType { get; set; }
        public string FilterMeAsNull { get; set; }
    }
}