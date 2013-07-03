using System.Data;
using System.IO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace AutoFilter.NHibernate.Tests
{
    /// <summary>
    /// Provides a base class for working with NHibernate in-memory style
    /// </summary>
    public abstract class BaseNHibernateTest
    {
        /// <summary>
        /// Gets or sets the current NHibernate session.
        /// </summary>
        protected ISession Session { get; set; }

        /// <summary>
        /// Holds the <see cref="ISessionFactory"/> for the process.
        /// </summary>
        protected static ISessionFactory SessionFactory { get; set; }

        /// <summary>
        /// Contains NHibernate configuration.
        /// </summary>
        protected static Configuration NHibernateConfiguration { get; set; }

        /// <summary>
        /// Indicates whether to open a transaction for the test or not
        /// </summary>
        protected virtual bool IsTransactional
        {
            get { return true; }
        }

        ///<summary>
        /// Test fixture level setup - creation of ISessioFactory if needed.
        ///</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            if (SessionFactory == null || MakeTestIndependent)
            {
                NHibernateConfiguration = GetNHibernateConfiguration();
                SessionFactory = NHibernateConfiguration.BuildSessionFactory();
            }


            DoFixtureSetup();
        }

        /// <summary>
        /// Clears the second level cache if it is enabled
        /// </summary>
        protected virtual void ClearNHibernate2edLevelCache()
        {
            SessionFactory.EvictQueries();

            foreach (var collectionMetadata in SessionFactory.GetAllCollectionMetadata())
                SessionFactory.EvictCollection(collectionMetadata.Key);

            foreach (var classMetadata in SessionFactory.GetAllClassMetadata())
                SessionFactory.EvictEntity(classMetadata.Key);
        }

        /// <summary>
        /// Override if you don't want to use the default in-memory database, and use something else instead.
        /// You should create here an NHibernate configuration object any way you want (probably using Fluently.Configure()...)
        /// </summary>
        protected virtual Configuration GetNHibernateConfiguration()
        {
            SQLiteConfiguration sqLiteConfiguration = SQLiteConfiguration.Standard.InMemory();
            if (ShowSql)
                sqLiteConfiguration = sqLiteConfiguration.ShowSql();
            return Fluently.Configure()
                .Database(sqLiteConfiguration)
                .Mappings(AddMappings)
                .ExposeConfiguration(AddCustomConfiguration).BuildConfiguration();
        }

        /// <summary>
        /// Runs test-fixture level cleanup code.
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (MakeTestIndependent)
            {
                SessionFactory.Dispose();
                SessionFactory = null;
                NHibernateConfiguration = null;
            }
            DoFixtureTearDown();
        }

        /// <summary>
        /// Test level setup - creation of ISession, and building the schema of the in-memory DB.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Session = SessionFactory.OpenSession();

            if (ExportSchemaToDbBeforeEachTest)
            {
                if (string.IsNullOrEmpty(SchemaExportOutputFilename))
                {
                    new SchemaExport(NHibernateConfiguration).Execute(ShowSql, true, false, Session.Connection, null);
                }
                else
                {
                    using (TextWriter outputFile = File.CreateText(SchemaExportOutputFilename))
                    {
                        new SchemaExport(NHibernateConfiguration).Execute(ShowSql, true, false, Session.Connection, outputFile);
                    }
                }
            }

            if (IsTransactional)
            {
                if (IsolationLevel != null)
                    Session.BeginTransaction(IsolationLevel.Value);
                else Session.BeginTransaction();
            }

            DoSetup();
        }

        /// <summary>
        /// Override if you don't want to create the database in the setup stage before each test.
        /// </summary>
        protected virtual bool ExportSchemaToDbBeforeEachTest
        {
            get { return true; }
        }

        /// <summary>
        /// Test level cleanup - disposes the NHibernate session.
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            DoTearDown();
            Session.Flush();
            if (Session.Transaction != null && Session.Transaction.IsActive)
                Session.Transaction.Rollback();
            Session.Dispose();
        }

        /// <summary>
        /// Override this if you want to force the test to create ISessionFactory on its own (slower, but more independent).
        /// </summary>
        protected virtual bool MakeTestIndependent
        {
            get { return false; }
        }

        /// <summary>
        /// Override if you want NHibernate's SQL commands to show in the output window.
        /// </summary>
        protected virtual bool ShowSql
        {
            get { return false; }
        }

        /// <summary>
        /// Override if you want to export the DDL performed in the schema export to a text writer.
        /// </summary>
        protected virtual string SchemaExportOutputFilename
        {
            get { return null; }
        }

        /// <summary>
        /// Transaction isolation level, if not supplied standard transation is opened
        /// </summary>
        protected virtual IsolationLevel? IsolationLevel
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Override to provide your own test-level setup.
        /// </summary>
        protected virtual void DoSetup()
        {
        }

        /// <summary>
        /// Override to provide your own test-fixture level setup.
        /// </summary>
        protected virtual void DoFixtureSetup()
        {
        }

        /// <summary>
        /// Override to provide your own test-fixture level cleanup code.
        /// </summary>
        protected virtual void DoFixtureTearDown()
        {
        }

        /// <summary>
        /// Override to provide your own test-level cleanup.
        /// </summary>
        protected virtual void DoTearDown()
        {
        }

        /// <summary>
        /// Override to add your own configuration to NHibernate.
        /// </summary>
        /// <param name="configuration">NHibernate configuration object to manipulate.</param>
        protected virtual void AddCustomConfiguration(Configuration configuration)
        {
        }

        /// <summary>
        /// Overrider to say which mappings should NHibernate work with.
        /// </summary>
        /// <param name="m">An <see cref="MappingConfiguration"/> to add mappings to.</param>
        /// <example>
        /// <code>
        /// m.FluentMappings.Add(typeof (TestEntityMap));
        /// </code>
        /// </example>
        protected abstract void AddMappings(MappingConfiguration m);
    }
}