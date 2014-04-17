using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Extensions;
using System.Data.Entity.Infrastructure.Annotations;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public static class EdmPropertyExtensions
    {
        public static ColumnModel ToColumnModel(this EdmProperty property, DbProviderManifest providerManifest, IDictionary<string, AnnotationValues> annotations = null)
        {
            Check.NotNull(property, "property");
            Check.NotNull(providerManifest, "providerManifest");

            var conceptualTypeUsage = providerManifest.GetEdmType(property.TypeUsage);
            var defaultStoreTypeUsage = providerManifest.GetStoreType(conceptualTypeUsage);

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
