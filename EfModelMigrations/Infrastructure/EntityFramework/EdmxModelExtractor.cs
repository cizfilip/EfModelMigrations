using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    internal class EdmxModelExtractor
    {
        public XDocument GetEdmxModel(DbContext context)
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
                    EdmxWriter.WriteEdmx(context, xmlWriter);
                }

                memoryStream.Position = 0;

                doc = XDocument.Load(memoryStream);
            }
            return doc;
        }

        public string GetEdmxModelAsString(DbContext context)
        {
            using (var writer = new StringWriter())
            {
                GetEdmxModel(context).Save(writer);
                return writer.ToString();
            }
        }
    }
}
