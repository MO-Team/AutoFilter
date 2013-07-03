using NUnit.Framework;

namespace AutoFilter.Tests
{
    [TestFixture]
    public class FilterEngineTest
    {
        [Test]
        public void BuildExpression_HasExpressionBuilderMapped_BuildeExpression()
        {
            //Arrange
            TestCatalogFilter filter = new TestCatalogFilter() { DataInsertionDate = null };
            filter.ProductName = "OMRI";
            FilterConfiguration filterConfiguration = new FilterConfiguration();
            filterConfiguration.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>();
            FilterEngine filterEngine = new FilterEngine(filterConfiguration);
            //Act
            var expression = filterEngine.BuildExpression<TestCatalogFilter, QueryHandlerTestClass>(filter);

            Assert.AreEqual("(Convert(\"OMRI\") == Convert(QueryHandlerTestClass.ProductName))",expression.Body.ToString());
        }

        [Test]
        public void EmptyFilter_HasExpressionBuilderMapped_BuildeExpression()
        {
            //Arrange
            TestCatalogFilter filter = new TestCatalogFilter() { DataInsertionDate = null };
            FilterConfiguration filterConfiguration = new FilterConfiguration();
            filterConfiguration.CreateFilter<TestCatalogFilter, QueryHandlerTestClass>();
            FilterEngine filterEngine = new FilterEngine(filterConfiguration);
            //Act
            var expression = filterEngine.BuildExpression<TestCatalogFilter, QueryHandlerTestClass>(filter);

            Assert.AreEqual( expression.Body.ToString(),"True");
        }

        [Test]
        [ExpectedException(typeof(FilterNotMappedException))]
        public void BuildExpression_FilterMappingNotExist_Throws()
        {
            //Arrange
            TestCatalogFilter filter = new TestCatalogFilter();
            filter.ProductName = "OMRI";
            FilterConfiguration filterConfiguration = new FilterConfiguration();
            FilterEngine filterEngine = new FilterEngine(filterConfiguration);
            //Act
            filterEngine.BuildExpression<TestCatalogFilter, QueryHandlerTestClass>(filter);
        }

    }
}
