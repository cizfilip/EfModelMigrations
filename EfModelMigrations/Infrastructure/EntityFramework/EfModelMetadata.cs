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

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public class EfModelMetadata
    {
        private EfModelMetadata () { }

        public EdmItemCollection EdmItemCollection { get; set; }
        public StoreItemCollection StoreItemCollection { get; set; }
        public EntityContainerMapping EntityContainerMapping { get; set; }
        public EntityContainer StoreEntityContainer { get; set; }
        public DbProviderManifest ProviderManifest { get; set; }
        public DbProviderInfo ProviderInfo { get; set; }


        public static EfModelMetadata Load(string edmx)
        {
            var metadata = XDocument.Parse(edmx).LoadEfModelMetadata();
            return new EfModelMetadata()
            {
                EdmItemCollection = metadata.Item1,
                StoreItemCollection = metadata.Item2,
                StoreEntityContainer = metadata.Item2.GetItems<EntityContainer>().Single(),
                EntityContainerMapping = metadata.Item3.GetItems<EntityContainerMapping>().Single(),
                ProviderManifest = GetProviderManifest(metadata.Item4),
                ProviderInfo = metadata.Item4
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
