using System;
using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Mapping;
using AutoFilter;
using NHibernate.Linq;
using NHibernate.Type;
using NUnit.Framework;

namespace AutoFilter.Tests.NHibernate
{
    [TestFixture]
    public class NHibernateQueriesTests : BaseNHibernateTest
    {
        protected override void AddMappings(MappingConfiguration m)
        {
            m.FluentMappings.Add<NHProductMap>();
        }
        
        [Test]
        public void MapBoolAsYesNoType_ConditionIsTrue_QueryResultsAreCorrect()
        {
            // Arrange:
            var config = new FilterConfiguration();
            config.CreateFilter<NHProductFilter, NHProduct>();
            var engine = new FilterEngine(config);
            Session.Save(new NHProduct { Id = 1, IsAvailable = false });
            Session.Save(new NHProduct { Id = 2, IsAvailable = true });
            Session.Flush();
            Session.Clear();

            // Act:
            var filter = new NHProductFilter { IsAvailable = true };
            var whereClause = engine.BuildExpression<NHProductFilter, NHProduct>(filter);
            var result = Session.Query<NHProduct>().Where(whereClause);

            // Assert:
            Assert.That(result.Single().Id, Is.EqualTo(2));
        }

        #region Test Entities and Maps

        public class NHProduct
        {
            public virtual int Id { get; set; }
            public virtual bool IsAvailable { get; set; }
        }

        public class NHProductMap : ClassMap<NHProduct>
        {
            public NHProductMap()
            {
                Id(p => p.Id).GeneratedBy.Assigned();
                Map(p => p.IsAvailable).CustomType<YesNoType>();
            }
        }

        public class NHProductFilter
        {
            public bool? IsAvailable { get; set; }
        }

        #endregion
    }
}
