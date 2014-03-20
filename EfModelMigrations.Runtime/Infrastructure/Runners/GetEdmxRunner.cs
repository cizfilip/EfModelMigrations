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
        public override void Run()
        {
            Return(GetEdmxModelAsString());
        }

        private XDocument GetEdmxModel()
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
    }
}
