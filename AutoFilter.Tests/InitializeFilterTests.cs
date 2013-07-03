using System;
using System.Linq;
using AutoFilter.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;

namespace AutoFilter.Tests
{
    [TestFixture]
    public class InitializeFilterTests
    {
        #region Properties

        private IFilterConfiguration _config;

        #endregion

        [SetUp]
        public void SetUp()
        {
            _config = new FilterConfiguration();
            TestCreateObjectFilter.TimesPerformed = 0;
            TestCreateSecondObjectFilter.TimesPerformed = 0;
            TestCreateCatalogFilter.TimesPerformed = 0;
            TestCreateEmptyFilter.TimesPerformed = 0;
            TestCreateDoubleFilter.TimesPerformed = 0;
            TestCreateUserFilter.TimesPerformed = 0;
            TestCreateMixFilter.TimesPerformed = 0;
            TestCreateAbstractUserFilter.TimesPerformed = 0;
            TestCreateGenericUserFilter<int>.TimesPerformed = 0;
            TestCreateNoDefaultCtorUserFilter.TimesPerformed = 0;
            TestCreateStructUserFilter.TimesPerformed = 0;
        }

        #region Add Tests

        [Test]
        public void Add_EmptyFilter_EmptyFiltersList()
        {
            // Arrange:

            // Act:
            InitializeFilter.On(_config)
                .Add(typeof (TestCreateEmptyFilter))
                .Apply();

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Add_CreationWithOneFilter_OneItemInFiltersList()
        {
            // Arrange:
            var expected = typeof(FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>);

            // Act:
            InitializeFilter.On(_config)
                .Add(typeof (TestCreateObjectFilter))
                .Apply();
            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void Add_CreationWithTwoFilters_TwoItemsInFiltersList()
        {
            // Arrange:
            var expected = new List<Type>
                               {
                                   typeof (FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>),
                                   typeof (FilterExpressionBuilder<TestCatalogFilter, QueryHandlerTestClass>)
                               };

            // Act:
            InitializeFilter.On(_config)
                .Add(typeof (TestCreateDoubleFilter))
                .Apply();

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Count(), Is.EqualTo(expected.Count()));
            expected.ForEach(t => Assert.That(results, Contains.Item(t)));
        }

        [Test]
        public void Add_RepeatFilter_OneShowPerObjectInFiltersList()
        {
            // Arrange:
            var expected = new List<Type>
                               {
                                   typeof (FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>),
                                   typeof (FilterExpressionBuilder<TestCatalogFilter, QueryHandlerTestClass>)
                               };

            // Act:
            InitializeFilter.On(_config)
                .Add(typeof (TestCreateObjectFilter))
                .Add(typeof (TestCreateDoubleFilter))
                .Apply();

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Count(), Is.EqualTo(expected.Count()));
            expected.ForEach(t => Assert.That(results, Contains.Item(t)));
        }

        [Test]
        public void Add_MapFilterObjectToTwoDifferentObjects_BothFiltersAdded()
        {
            // Arrange:
            var expected = new List<Type>
                               {
                                   typeof (FilterExpressionBuilder<TestUserFilter, TestUserFilterQuery>),
                                   typeof (FilterExpressionBuilder<TestUserFilter, TestObjectFilterQuery>)
                               };

            // Act:
            InitializeFilter.On(_config)
                .Add(typeof (TestCreateMixFilter))
                .Apply();

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Count(), Is.EqualTo(expected.Count()));
            expected.ForEach(t => Assert.That(results, Contains.Item(t)));
        }

        #region Generic Add Tests

        [Test]
        public void GenericAdd_CreationWithOneFilter_ReturnOneItemInList()
        {
            // Arrange:
            var expected = typeof (FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>);

            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateObjectFilter>()
                .Apply();
            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.Single(), Is.EqualTo(expected));
        }

        #endregion

        #endregion

        #region Exclude Tests

        [Test]
        public void Exclude_AddTwoFiltersExcludedItemNotAdded_AllAddedItemsInFiltersList()
        {
            // Arrange:
            var expected = new List<Type>
                               {
                                   typeof (FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>)
                               };

            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateObjectFilter>()
                .Add<TestCreateCatalogFilter>()
                .Exclude(typeof (TestCreateCatalogFilter))
                .Apply();

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Count(), Is.EqualTo(expected.Count()));
            expected.ForEach(t => Assert.That(results, Contains.Item(t)));
        }

        [Test]
        public void GenericExclude_NoFiltersAdd_EmptyFilterList()
        {
            // Arrange:

            // Act:
            InitializeFilter.On(_config)
                .Exclude<TestCreateObjectFilter>()
                .Apply();

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GenericExclude_AddOneFilterExcludedFilterWasNotAdd_OneItemInFiltersList()
        {
            // Arrange:
            var expected = typeof(FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>);

            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateObjectFilter>()
                .Exclude<TestCreateCatalogFilter>()
                .Apply();

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void GenericExclude_AddTwoFiltersExcludeOneOfThem_ExcludedItemNotInFiltersList()
        {
            // Arrange:
            var expected = typeof(FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>);

            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateObjectFilter>()
                .Add<TestCreateCatalogFilter>()
                .Exclude<TestCreateCatalogFilter>()
                .Apply();

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void GenericExclude_AddAndExcludeOneFilter_EmptyFiltersList()
        {
            // Arrange:

            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateObjectFilter>()
                .Exclude<TestCreateObjectFilter>()
                .Apply();

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GenericExclude_ExcludeTwoAddedFilters_BothFiltersNotInFiltersList()
        {
            // Arrange:
            
            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateObjectFilter>()
                .Add<TestCreateCatalogFilter>()
                .Exclude<TestCreateObjectFilter>()
                .Exclude<TestCreateCatalogFilter>()
                .Apply();

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results, Is.Empty);
        }

        [Test]
        public void GenericExclude_RepeatSimilarFilterExclude_FilterNotInFilterList()
        {
            // Arrange:
            var expected = typeof(FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>);

            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateObjectFilter>()
                .Add<TestCreateCatalogFilter>()
                .Exclude<TestCreateCatalogFilter>()
                .Exclude<TestCreateCatalogFilter>()
                .Apply();

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.Single(), Is.EqualTo(expected));
        }

        #endregion

        #region AddFromAssemblyOf Tests

        [Test]
        public void AddFromAssemblyOf_TypeIsICreateFilter_LoadAllICreateFilterFromAssembly()
        {
            // Arrange:
            const int expectedRunCount = 6;
            var expected = new List<Type>
                               {
                                   typeof (FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>),
                                   typeof (FilterExpressionBuilder<TestCatalogFilter, QueryHandlerTestClass>),
                                   typeof (FilterExpressionBuilder<TestUserFilter, TestUserFilterQuery>),
                                   typeof (FilterExpressionBuilder<TestUserFilter, TestObjectFilterQuery>)
                               };

            // Act:
            InitializeFilter.On(_config)
                .AddFromAssemblyOf(typeof (TestCreateEmptyFilter))
                .Apply();

            var actualRunCount = TestCreateObjectFilter.TimesPerformed +
                                 TestCreateSecondObjectFilter.TimesPerformed +
                                 TestCreateCatalogFilter.TimesPerformed +
                                 TestCreateEmptyFilter.TimesPerformed +
                                 TestCreateDoubleFilter.TimesPerformed +
                                 TestCreateUserFilter.TimesPerformed +
                                 TestCreateAbstractUserFilter.TimesPerformed +
                                 TestCreateGenericUserFilter<int>.TimesPerformed +
                                 TestCreateNoDefaultCtorUserFilter.TimesPerformed +
                                 TestCreateStructUserFilter.TimesPerformed;

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(actualRunCount, Is.EqualTo(expectedRunCount));
            Assert.That(results.Count(), Is.EqualTo(expected.Count()));
            expected.ForEach(t => Assert.That(results, Contains.Item(t)));
        }

        [Test]
        public void GenericAddFromAssemblyOf_TypeIsICreateFilter_LoadAllICreateFilterFromAssembly()
        {
            // Arrange:
            const int expectedRunCount = 6;
            var expected = new List<Type>
                               {
                                   typeof (FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>),
                                   typeof (FilterExpressionBuilder<TestCatalogFilter, QueryHandlerTestClass>),
                                   typeof (FilterExpressionBuilder<TestUserFilter, TestUserFilterQuery>),
                                   typeof (FilterExpressionBuilder<TestUserFilter, TestObjectFilterQuery>)
                               };

            // Act:
            InitializeFilter.On(_config)
                .AddFromAssemblyOf<TestCreateEmptyFilter>()
                .Apply();

            var actualRunCount = TestCreateObjectFilter.TimesPerformed +
                                 TestCreateSecondObjectFilter.TimesPerformed +
                                 TestCreateCatalogFilter.TimesPerformed +
                                 TestCreateEmptyFilter.TimesPerformed +
                                 TestCreateDoubleFilter.TimesPerformed +
                                 TestCreateUserFilter.TimesPerformed +
                                 TestCreateAbstractUserFilter.TimesPerformed +
                                 TestCreateGenericUserFilter<int>.TimesPerformed +
                                 TestCreateNoDefaultCtorUserFilter.TimesPerformed +
                                 TestCreateStructUserFilter.TimesPerformed;

            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(actualRunCount, Is.EqualTo(expectedRunCount));
            Assert.That(results.Count(), Is.EqualTo(expected.Count()));
            expected.ForEach(t => Assert.That(results, Contains.Item(t)));
        }
        
        #endregion

        #region Apply Tests

        [Test]
        public void Apply_OneFilter_ReturnFilterConfiguration()
        {
            // Arrange:

            // Act:
            var configuration = InitializeFilter.On(_config)
                                    .Add<TestCreateObjectFilter>()
                                    .Add<TestCreateSecondObjectFilter>()
                                    .Apply();

            // Assert:
            Assert.That(configuration, Is.Not.Null);
            Assert.That(configuration, Is.InstanceOf<IFilterConfiguration>());
        }

        [Test]
        public void Apply_DifferentFiltersSameCreate_FilterExistsOnceInFiltersList()
        {
            // Arrange:
            var expected = typeof(FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>);

            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateObjectFilter>()
                .Add<TestCreateSecondObjectFilter>()
                .Apply();
            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void Apply_DifferentFiltersSameCreateOneExcluded_FilterExistsInFilterList()
        {
            // Arrange:
            var expected = typeof(FilterExpressionBuilder<TestObjectFilter, TestObjectFilterQuery>);

            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateObjectFilter>()
                .Add<TestCreateSecondObjectFilter>()
                .Exclude<TestCreateSecondObjectFilter>()
                .Apply();
            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void Apply_AddAbstractClass_FilterIsNotInFiltersList()
        {
            // Arrange:
            
            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateAbstractUserFilter>()
                .Apply();
            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Apply_AddGenericClass_FilterIsNotInFiltersList()
        {
            // Arrange:

            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateGenericUserFilter<int>>()
                .Apply();
            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Apply_AddClassWithNoDefaultCtor_FilterIsNotInFiltersList()
        {
            // Arrange:

            // Act:
            InitializeFilter.On(_config)
                .Add<TestCreateNoDefaultCtorUserFilter>()
                .Apply();
            var results = _config.GetFilters().Select(f => f.GetType());

            // Assert:
            Assert.That(results, Is.Empty);
        }

        #endregion
    }
}