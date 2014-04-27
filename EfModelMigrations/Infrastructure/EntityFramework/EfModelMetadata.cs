using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework.Edmx;
using EfModelMigrations.Extensions;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Xml.Linq;
using System.Linq;
using System.Data.Entity;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using EfModelMigrations.Exceptions;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    //TODO: idealne dopsat dokumentaci ze muze vyhazovat exceptiony...
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

        private string edmx;
        internal string Edmx
        {
            get
            {
                return edmx;
            }
        }

        private EfModelMetadata(string edmx) 
        {
            this.edmx = edmx;
        }

        public EdmItemCollection EdmItemCollection { get; private set; }
        public StoreItemCollection StoreItemCollection { get; private set; }
        public StorageMappingItemCollection StorageMappingItemCollection { get; private set; }
        public DbProviderManifest ProviderManifest { get; private set; }
        public DbProviderInfo ProviderInfo { get; private set; }

        private EntityContainer entityContainer;
        public EntityContainer EntityContainer
        {
            get
            {
                if (entityContainer == null)
                {
                    entityContainer = EdmItemCollection.GetItems<EntityContainer>().Single();
                }

                return entityContainer;
            }
        }

        private EntityContainer storeEntityContainer;
        public EntityContainer StoreEntityContainer
        {
            get
            {
                if (storeEntityContainer == null)
                {
                    storeEntityContainer = StoreItemCollection.GetItems<EntityContainer>().Single();
                }

                return storeEntityContainer;
            }
        }

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

        private IEnumerable<AssociationType> storeAssociatonTypes;
        public IEnumerable<AssociationType> StoreAssociatonTypes
        {
            get
            {
                if (storeAssociatonTypes == null)
                {
                    storeAssociatonTypes = StoreItemCollection.GetItems<AssociationType>();
                }

                return storeAssociatonTypes;
            }
        }

        public EntityType GetEntityTypeForClass(string className)
        {
            Check.NotEmpty(className, "className");
            
            return EdmItemCollection.GetItems<EntityType>().Single(e => e.Name.EqualsOrdinal(className));
        }

        public EntityTypeMapping GetEntityTypeMappingForClass(string className)
        {
            Check.NotEmpty(className, "className");

            return EntityTypeMappings
                .Single(t => 
                    t.IsHierarchyMapping ? 
                    t.IsOfEntityTypes.Single().Name.EqualsOrdinal(className) : 
                    t.EntityType.Name.EqualsOrdinal(className));
        }

        //TODO: hint jak handlovat mapping fragmenty je v efmodeldifferu metoda FindRenamedMappedColumns
        public EntitySet GetStoreEntitySetForClass(string className)
        {
            Check.NotEmpty(className, "className");

            return GetEntityTypeMappingForClass(className)
                .Fragments
                .Single()
                .StoreEntitySet;
        }

        public ScalarPropertyMapping GetScalarPropertyMappingForProperty(string className, string propertyName)
        {
            Check.NotEmpty(className, "className");
            Check.NotEmpty(propertyName, "propertyName");

            return GetEntityTypeMappingForClass(className)
                .Fragments
                .SelectMany(f => f.PropertyMappings)
                .OfType<ScalarPropertyMapping>()
                .Single(p => p.Property.Name.EqualsOrdinal(propertyName));
        }

        public static EfModelMetadata Load(string edmx)
        {
            Check.NotNullOrEmpty(edmx, "edmx");

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

            return new EfModelMetadata(edmx)
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
