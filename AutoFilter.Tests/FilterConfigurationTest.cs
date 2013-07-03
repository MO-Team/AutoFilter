using System;
using System.Collections.Generic;
using System.Linq;
using AutoFilter;
using NUnit.Framework;
using AutoFilter.Tests.NHibernate;

namespace AutoFilter.Tests
{
    [TestFixture]
    public class FilterConfigurationTest
    {
        #region CreateFilter Tests

        [Test]
        public void CreateFilter_HasExpressionBuilderMapped_AddNewConfiguration()
        {
            //Arrange
            var filterConfiguration = new FilterConfiguration();

            //Act
            filterConfiguration.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>();

            //Assert
            Assert.IsNotNull(filterConfiguration.GetFilter<TestCatalogFilter, QueryHandlerTestClass>());
        }

        [Test]
        public void CreateFilter_FilterMappingNotExist_ReturnNull()
        {
            //Arrange
            var filterConfiguration = new FilterConfiguration();
            //Assert
            Assert.IsNull(filterConfiguration.GetFilter<TestCatalogFilter, QueryHandlerTestClass>());

        }

        [Test]
        public void CreateFilter_FilterMappingExist_UseSameMapping()
        {
            //Arrange
            var filterEngine = new FilterConfiguration();
            var expected = filterEngine.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>();
            //Act
            var actual = filterEngine.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>();

            //Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetFilters Tests

        [Test]
        public void GetFilters_NoFilters_ReturnEmptyList()
        {
            // Arrange:
            var filterEngine = new FilterConfiguration();

            // Act:
            var results = filterEngine.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GetFilters_OneFilter_ReturnOneItemInList()
        {
            // Arrange:
            var filterEngine = new FilterConfiguration();
            filterEngine.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>();
            var expected = typeof(FilterExpressionBuilder<TestCatalogFilter, QueryHandlerTestClass>);

            // Act:
            var results = filterEngine.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void GetFilters_TwoFilter_ReturnTwoItemsInList()
        {
            // Arrange:
            var filterEngine = new FilterConfiguration();
            filterEngine.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>();
            filterEngine.CreateFilter<TestObjectFilter, TestObjectFilterQuery>();

            var expected = new List<Type>
                               {
                                   typeof (FilterExpressionBuilder<TestCatalogFilter, QueryHandlerTestClass>),
                                   typeof (FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>)
                               };

            // Act:
            var results = filterEngine.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Count(), Is.EqualTo(2));
            expected.ForEach(t => Assert.That(results, Contains.Item(t)));
        }

        #endregion

        #region AssertConfigurationIsValid Tests

        [Test]
        public void AssertConfigurationIsValid_NoFiltersAdded_AllOk()
        {
            // Arrange:
            var config = new FilterConfiguration();

            // Act:
            config.AssertConfigurationIsValid();
        }

        [Test]
        public void AssertConfigurationIsValid_OneValidFilterMappingAdded_AllOk()
        {
            // Arrange:
            var config = new FilterConfiguration();
            config.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>()
                    .Ignore(f => f.EditionName);

            // Act:
            config.AssertConfigurationIsValid();
        }

        [Test]
        public void AssertConfigurationIsValid_MoreThanOneValidFilterMappingAdded_AllOk()
        {
            // Arrange:
            var config = new FilterConfiguration();
            config.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>()
                    .Ignore(f => f.EditionName);
            config.CreateFilter<TestObjectFilter, TestObjectFilterQuery>();

            // Act:
            config.AssertConfigurationIsValid();
        }

        [Test]
        public void AssertConfigurationIsValid_OneFilterWithMissedPropertyMappingAdded_Exception()
        {
            // Arrange:
            var config = new FilterConfiguration();
            config.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>();

            // Act:
            TestDelegate action = () => config.AssertConfigurationIsValid();

            // Assert:
            Assert.That(action, Throws.InstanceOf<FilterPropertyMissingMappingException>());
        }

        [Test]
        public void AssertConfigurationIsValid_OneValidMappingAndOneWithMissedPropertyMappingAdded_Exception()
        {
            // Arrange:
            var config = new FilterConfiguration();
            config.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>();
            config.CreateFilter<TestObjectFilter, TestObjectFilterQuery>();

            // Act:
            TestDelegate action = () => config.AssertConfigurationIsValid();

            // Assert:
            Assert.That(action, Throws.InstanceOf<FilterPropertyMissingMappingException>());
        }

        #endregion
    }
}