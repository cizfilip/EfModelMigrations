using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Extensions
{
    internal static class MetadataWorkspaceExtensions
    {
        //TODO: fail - vsechno se to pta SSpace takze to rovnou pocita s tim ze jmeno tridy == jmeno tabulky.... fail
        //tak asi neni fail alespon pro mapovani jmena GetTableNameForClass je jadro pudla v BaseEntitySets - nejspis


        //TODO: rewrite using this: http://romiller.com/2013/09/24/ef-code-first-mapping-between-types-tables/

        //TODO: co kdyz se tridy nenajdou v metadatech... - nemelo by nejspis nastat - promyslet

        public static string GetTableNameForClassName(this MetadataWorkspace metadata, string className)
        {
            EntitySetBase et = metadata.GetItemCollection(DataSpace.SSpace)
                .GetItems<EntityContainer>()
                .Single()
                .BaseEntitySets
                .Where(x => EqualsIgnoreCase(x.Name, className)) 
                .Single();

            String tableName = String.Concat(et.MetadataProperties["Schema"].Value, ".", et.MetadataProperties["Table"].Value);

            return tableName;
        }

        public static IEnumerable<EdmProperty> GetTableColumnsForClass(this MetadataWorkspace metadata, string className)
        {
            EntityType storageEntityType = metadata.GetItems(DataSpace.SSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Where(x => EqualsIgnoreCase(x.Name, className))  
                .Single();

            return storageEntityType.Properties;
        }

        public static IEnumerable<EdmMember> GetTableKeyColumnsForClass(this MetadataWorkspace metadata, ClassCodeModel classModel)
        {
            EntityType storageEntityType = metadata.GetItems(DataSpace.SSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>().Where(x => EqualsIgnoreCase(x.Name, classModel.Name))
                .Single();

            return storageEntityType.KeyMembers;
        }


        //TODO: ud2lat jako extension metodu a pouyivat i jinde.... např v FindMigrationsToApplyOrRevert se používa nonstop equal ordinal
        private static bool EqualsIgnoreCase(string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
