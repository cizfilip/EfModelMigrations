using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EfModelMigrations.Infrastructure.EntityFramework.Edmx
{
    internal static class XDocumentExtensions
    {
        public static EfModelMetadata GetEfModelMetadata(
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

            return new EfModelMetadata()
            {
                EdmItemCollection = edmItemCollection,
                StoreItemCollection = storeItemCollection,
                StoreEntityContainer = storeItemCollection.GetItems<EntityContainer>().Single(),
                EntityContainerMapping = storageMappingItemCollection.GetItems<EntityContainerMapping>().Single(),
                ProviderManifest = GetProviderManifest(providerInfo),
                ProviderInfo = providerInfo
            };
        }

        private static DbProviderManifest GetProviderManifest(DbProviderInfo providerInfo)
        {
            var providerFactory = DbConfiguration.DependencyResolver.GetService<DbProviderFactory>(providerInfo.ProviderInvariantName);

            return providerFactory.GetProviderServices().GetProviderManifest(providerInfo.ProviderManifestToken);
        }

        private static DbProviderServices GetProviderServices(this DbProviderFactory factory)
        {
            // The EntityClient provider invariant name is not normally registered so we can't use
            // the normal method for looking up this factory.
            if (factory is EntityProviderFactory)
            {
                //TODO: Hack - EF zde pouziva return EntityProviderServices.Instance; ale to jest internal, 
                //ovsem podle zdrojaku udela nasledujici radka to same
                return (factory as IServiceProvider).GetService(typeof(DbProviderServices)) as DbProviderServices;
            }

            var invariantName = DbConfiguration.DependencyResolver.GetService<IProviderInvariantName>(factory);

            return DbConfiguration.DependencyResolver.GetService<DbProviderServices>(invariantName.Name);
        }
    }

    internal class EfModelMetadata
    {
        public EdmItemCollection EdmItemCollection { get; set; }
        public StoreItemCollection StoreItemCollection { get; set; }
        public EntityContainerMapping EntityContainerMapping { get; set; }
        public EntityContainer StoreEntityContainer { get; set; }
        public DbProviderManifest ProviderManifest { get; set; }
        public DbProviderInfo ProviderInfo { get; set; }
    }
}
