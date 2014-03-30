using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using EfModelMigrations.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure.Annotations;
using EfModelMigrations.Exceptions;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    //TODO: vyhazovat zde vyjimky nebo vracet null?
    public sealed class EfModel
    {
        public EfModelMetadata Metadata { get; private set; }

        public EfModel(string edmx)
        {
            this.Metadata = EfModelMetadata.Load(edmx);
        }

        internal EfModel(EfModelMetadata metadata)
        {
            this.Metadata = metadata;
        }

        //TODO: nejspis nebude fungovat pri dedicnosti - EntityType bude mit vice MappingFragmentu
        public string GetTableNameForClass(string className, bool includeSchema = true)
        {
            Check.NotEmpty(className, "className");

            try
            {
                var storeEntitySet = Metadata.EntityTypeMappings
                    .Single(t => t.EntityType.Name.EqualsOrdinal(className))
                    .Fragments
                    .Single()
                    .StoreEntitySet;

                if (includeSchema)
                {
                    return string.Concat(storeEntitySet.Schema, ".", storeEntitySet.Table);
                }
                else
                {
                    return storeEntitySet.Table;
                }
            }
            catch (Exception e)
            {
                throw new EfModelException(string.Format("Cannot find table name for class {0}", className), e); //TODO: string do resourcu
                throw;
            }
        }

        public EntityType GetEntityTypeForClass(string className)
        {
            Check.NotEmpty(className, "className");

            try
            {
                return Metadata.EdmItemCollection.GetItems<EntityType>().Single(e => e.Name.EqualsOrdinal(className));
            }
            catch (Exception e)
            {
                throw new EfModelException(string.Format("Cannot find entity type for class {0}", className), e); //TODO: string do resourcu
            }
        }

        public ColumnModel GetColumnModelForProperty(string className, string propertyName)
        {
            Check.NotEmpty(className, "className");
            Check.NotEmpty(propertyName, "propertyName");

            try
            {
                var storeProperty = Metadata.EntityTypeMappings
                    .Single(t => t.EntityType.Name.EqualsOrdinal(className))
                    .Fragments
                    .SelectMany(f => f.PropertyMappings)
                    .OfType<ScalarPropertyMapping>()
                    .Single(p => p.Property.Name.EqualsOrdinal(propertyName))
                    .Column;

                return ConvertEdmPropertyToColumnModel(storeProperty);
            }
            catch (Exception e)
            {
                throw new EfModelException(string.Format("Cannot get ColumnModel for property {0} in class {1}", propertyName, className), e); //TODO: string do resourcu
            }
        }



        public ColumnModel ConvertEdmPropertyToColumnModel(EdmProperty property, IDictionary<string, AnnotationValues> annotations = null)
        {
            var conceptualTypeUsage = Metadata.ProviderManifest.GetEdmType(property.TypeUsage);
            var defaultStoreTypeUsage = Metadata.ProviderManifest.GetStoreType(conceptualTypeUsage);

            var column = new ColumnModel(property.PrimitiveType.PrimitiveTypeKind, conceptualTypeUsage)
            {
                Name
                    = property.Name,
                IsNullable
                    = !property.Nullable ? false : (bool?)null,
                StoreType
                    = !property.TypeName.EqualsOrdinalIgnoreCase(defaultStoreTypeUsage.EdmType.Name)
                        ? property.TypeName
                        : null,
                IsIdentity
                    = property.IsStoreGeneratedIdentity
                      && EfModelMetadata.ValidIdentityTypes.Contains(property.PrimitiveType.PrimitiveTypeKind),
                IsTimestamp
                    = property.PrimitiveType.PrimitiveTypeKind == PrimitiveTypeKind.Binary
                      && property.MaxLength == 8
                      && property.IsStoreGeneratedComputed,
                IsUnicode
                    = property.IsUnicode == false ? false : (bool?)null,
                IsFixedLength
                    = property.IsFixedLength == true ? true : (bool?)null,
                Annotations
                    = annotations
            };

            Facet facet;

            if (property.TypeUsage.Facets.TryGetValue(DbProviderManifest.MaxLengthFacetName, true, out facet)
                && !facet.IsUnbounded
                && !facet.Description.IsConstant)
            {
                column.MaxLength = (int?)facet.Value;
            }

            if (property.TypeUsage.Facets.TryGetValue(DbProviderManifest.PrecisionFacetName, true, out facet)
                && !facet.IsUnbounded
                && !facet.Description.IsConstant)
            {
                column.Precision = (byte?)facet.Value;
            }

            if (property.TypeUsage.Facets.TryGetValue(DbProviderManifest.ScaleFacetName, true, out facet)
                && !facet.IsUnbounded
                && !facet.Description.IsConstant)
            {
                column.Scale = (byte?)facet.Value;
            }

            return column;
        }

    }
}
