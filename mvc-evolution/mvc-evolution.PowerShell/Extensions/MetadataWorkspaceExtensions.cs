using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc_evolution.PowerShell.Extensions
{
    internal static class MetadataWorkspaceExtensions
    {
        public static string GetTableNameForType(this MetadataWorkspace metadata, Type entityType)
        {
            EntitySetBase et = metadata.GetItemCollection(DataSpace.SSpace)
                .GetItems<EntityContainer>()
                .Single()
                .BaseEntitySets
                .Where(x => x.Name == entityType.Name) //TODO: Equals ignore case !
                .Single();

            String tableName = String.Concat(et.MetadataProperties["Schema"].Value, ".", et.MetadataProperties["Table"].Value);

            return tableName;
        }

        public static IEnumerable<EdmProperty> GetTableColumnsForType(this MetadataWorkspace metadata, Type entityType)
        {
            EntityType storageEntityType = metadata.GetItems(DataSpace.SSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Where(x => x.Name == entityType.Name)  //TODO: Equals ignore case !
                .Single();

            return storageEntityType.Properties;
        }

        public static IEnumerable<EdmMember> GetTableKeyColumnsForType(this MetadataWorkspace metadata, Type entityType)
        {
            EntityType storageEntityType = metadata.GetItems(DataSpace.SSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>().Where(x => x.Name == entityType.Name)
                .Single();

            return storageEntityType.KeyMembers;
        }

    }
}
