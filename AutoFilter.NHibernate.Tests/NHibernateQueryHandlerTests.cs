using System;
using System.Linq;
using System.Linq.Expressions;
using AutoFilter.Tests;
using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;
using NUnit.Framework;

namespace AutoFilter.NHibernate.Tests
{
    [TestFixture]
    public class NHibernateQueryHandlerTests : BaseNHibernateTest
    {
        NHibernateQueryHandler<QueryHandlerTestClass> QueryHandler { get; set; }

        #region NHib

        protected override bool MakeTestIndependent
        {
            get
            {
                return true;
            }
        }

        protected override void AddMappings(MappingConfiguration m)
        {
            m.FluentMappings.Add<QueryHandlerTestClassMap>();

        }

        #endregion

        #region Test Setup & TearDown

        protected bool DataWasInserted { get; set; }



        protected override void DoSetup()
        {
            InsertData();
            QueryHandler = new NHibernateQueryHandler<QueryHandlerTestClass>(Session);
        }

        #endregion

        #region ExecuteQuery Tests

        [Test]
        public void ExecuteQuery_QueryWithLinq_ExecutesTheQuery()
        {
            var query1 = from pm in QueryHandler.CreateFilter<QueryHandlerTestClass>()
                         where pm.ProductName.Contains("test") 
                         orderby pm.ProductName
                         select pm;
            var result1 = QueryHandler.Execute(query1).ToList();

            Assert.AreEqual(10,result1.Count);
        }


        [Test]
        public void ExecuteQuery_QueryWithLambdaExpression_ExecutesTheQuery()
        {
            Expression<Func<QueryHandlerTestClass, bool>> query =
                pm => pm.DataInsertionDate
                      > new DateTime(1973, 11, 4);
            var result = QueryHandler.Execute(query).ToList();

            Assert.AreEqual(6, result.Count);
        }



        [Test]
        public void ExecuteQuery_QueryWithExpressionTree_ExecutesTheQuery()
        {
            var filter = new TestCatalogFilter();
            filter.DataInsertionDate = new DateRange(new DateTime(1974,1,1),new DateTime(1976,12,12));

            var expressionBuilder = new FilterExpressionBuilder<TestCatalogFilter, QueryHandlerTestClass>();
            var expr = expressionBuilder.BuildExpression(filter);

            var result = QueryHandler.Execute<QueryHandlerTestClass>(expr).ToList();

            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void ExecuteQuery_QueryWithEmptyFilter_ExecutesTheQuery()
        {
            var filter = new TestCatalogFilter();


            var expressionBuilder = new FilterExpressionBuilder<TestCatalogFilter, QueryHandlerTestClass>();
            var expr = expressionBuilder.BuildExpression(filter);

            var result = QueryHandler.Execute(expr).ToList();

            Assert.AreEqual(10, result.Count);
        }

        #endregion

        #region Help Methods

        private void InsertData()
        {
            Console.WriteLine("Start insering data...");

            for (int i = 0; i < 10; i++)
            {
                var product = new QueryHandlerTestClass();
                product.ProductName = "TestProduct-" + i.ToString();
                product.DataInsertionDate = new DateTime(1970+i,11,4);
                Session.Save(product);
            }


            Session.Flush();
            Session.Clear();
            if (Session.Transaction.IsActive)
            {
                Session.Transaction.Commit();
            }

            Console.WriteLine("Finish insering data...");
            Console.WriteLine("------------------------------");
            Console.WriteLine();
            Console.WriteLine();
        }

        #endregion

    }

 
    public class QueryHandlerTestClassMap : ClassMap<QueryHandlerTestClass>
    {
        public QueryHandlerTestClassMap()
        {
            Id(x => x.IdAtSource).GeneratedBy.Native();
            Map(x => x.ProductName);
            Map(x => x.DataInsertionDate);
        }
    }

    
}

