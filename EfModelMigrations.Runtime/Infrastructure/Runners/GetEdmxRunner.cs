using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EfModelMigrations.Runtime.Infrastructure.Runners
{
    [Serializable]
    internal class GetEdmxRunner : BaseRunner
    {
           

        private DbMigrationsConfiguration dbConfiguration;
        protected DbMigrationsConfiguration DbConfiguration
        {
            get
            {
                if (dbConfiguration == null)
                {
                    dbConfiguration = CreateInstance<DbMigrationsConfiguration>(Configuration.EfMigrationsConfigurationType);
                }
                return dbConfiguration;
            }
        }

        private DbContext dbContext;
        protected DbContext DbContext
        {
            get
            {
                if (dbContext == null)
                {
                    dbContext = CreateInstance<DbContext>(DbConfiguration.ContextType);
                }
                return dbContext;
            }
        }

       

        protected XDocument GetEdmxModel()
        {
            XDocument doc;
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(
                    memoryStream, new XmlWriterSettings
                    {
                        Indent = true
                    }))
                {
                    EdmxWriter.WriteEdmx(DbContext, xmlWriter);
                }

                memoryStream.Position = 0;

                doc = XDocument.Load(memoryStream);
            }
            return doc;
        }

        private string GetEdmxModelAsString()
        {
            using (var writer = new StringWriter())
            {
                GetEdmxModel().Save(writer);
                return writer.ToString();
            }
        }

        public override void Run()
        {
            Return(GetEdmxModelAsString());
        }
    }
}
