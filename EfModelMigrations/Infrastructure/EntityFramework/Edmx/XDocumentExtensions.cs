using System;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Xml.Linq;

namespace EfModelMigrations.Infrastructure.EntityFramework.Edmx
{
    internal static class XDocumentExtensions
    {
        public static Tuple<EdmItemCollection, StoreItemCollection, StorageMappingItemCollection, DbProviderInfo> LoadEfModelMetadata(
            this XDocument model)
        {
            var edmItemCollection
                = new EdmItemCollection(
                    new[]
                        {
                            model.Descendants(EdmXNames.Csdl.SchemaNames).Single().CreateReader()
                        });

            var ssdlSchemaElement = model.Descendants(EdmXNames.Ssdl.SchemaNames).Single();

            var providerInfo = new DbProviderInfo(
                ssdlSchemaElement.ProviderAttribute(),
                ssdlSchemaElement.ProviderManifestTokenAttribute());

            var storeItemCollection
                = new StoreItemCollection(
                    new[]
                        {
                            ssdlSchemaElement.CreateReader()
                        });

            var storageMappingItemCollection = new StorageMappingItemCollection(
                edmItemCollection,
                storeItemCollection,
                new[] { new XElement(model.Descendants(EdmXNames.Msl.MappingNames).Single()).CreateReader() });

            return Tuple.Create(edmItemCollection, storeItemCollection, storageMappingItemCollection, providerInfo);
        }

        
    }

    
}
