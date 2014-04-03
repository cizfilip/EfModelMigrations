using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.EdmExtensions
{
    public static class MetadataItemExtensions
    {
        internal const string CustomAnnotationNamespace = "http://schemas.microsoft.com/ado/2013/11/edm/customannotation";
        internal const string CustomAnnotationPrefix = CustomAnnotationNamespace + ":";
        internal const string IndexAnnotationWithPrefix = CustomAnnotationPrefix + IndexAnnotation.AnnotationName;

        public static IEnumerable<MetadataProperty> Annotations(this MetadataItem item)
        {
            return item.MetadataProperties.Where(m => m.IsAnnotation);
        }

        public static IEnumerable<IndexAnnotation> IndexAnnotations(this MetadataItem item)
        {
            return Annotations(item).Where(a => a.Name == IndexAnnotationWithPrefix)
                .Select(s => s.Value)
                .OfType<IndexAnnotation>();
        }
    }
}
