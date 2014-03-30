using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework.Edmx;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Xml.Linq;
using System.Linq;
using System.Data.Entity;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public sealed class EfModelMetadata
    {
        public static readonly PrimitiveTypeKind[] ValidIdentityTypes =
        {
            PrimitiveTypeKind.Byte,
            PrimitiveTypeKind.Decimal,
            PrimitiveTypeKind.Guid,
            PrimitiveTypeKind.Int16,
            PrimitiveTypeKind.Int32,
            PrimitiveTypeKind.Int64
        };

        private EfModelMetadata() { }

        public EdmItemCollection EdmItemCollection { get; private set; }
        public StoreItemCollection StoreItemCollection { get; private set; }
        public StorageMappingItemCollection StorageMappingItemCollection { get; private set; }
        public DbProviderManifest ProviderManifest { get; private set; }
        public DbProviderInfo ProviderInfo { get; private set; }

        private EntityContainerMapping entityContainerMapping;
        public EntityContainerMapping EntityContainerMapping
        {
            get
            {
                if (entityContainerMapping == null)
                {
                    entityContainerMapping = StorageMappingItemCollection.GetItems<EntityContainerMapping>().Single();
                }

                return entityContainerMapping;
            }
        }

        private IEnumerable<EntityTypeMapping> entityTypeMappings;
        public IEnumerable<EntityTypeMapping> EntityTypeMappings
        {
            get
            {
                if (entityTypeMappings == null)
                {
                    entityTypeMappings = EntityContainerMapping
                        .EntitySetMappings
                        .SelectMany(m => m.EntityTypeMappings);
                }

                return entityTypeMappings;
            }
        }

        //TODO: upravit EdmxNames tak abych ho ideálně mohl z projektu úplně vypustit - jelikož z něho využívám jen to co je v této metodě
        public static EfModelMetadata Load(string edmx)
        {
            var model = XDocument.Parse(edmx);

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
                StorageMappingItemCollection = storageMappingItemCollection,
                ProviderManifest = GetProviderManifest(providerInfo),
                ProviderInfo = providerInfo
            };
        }

        private static DbProviderManifest GetProviderManifest(DbProviderInfo providerInfo)
        {
            var providerFactory = DbConfiguration.DependencyResolver.GetService<DbProviderFactory>(providerInfo.ProviderInvariantName);

            return GetProviderServices(providerFactory).GetProviderManifest(providerInfo.ProviderManifestToken);
        }

        private static DbProviderServices GetProviderServices(DbProviderFactory factory)
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
}
