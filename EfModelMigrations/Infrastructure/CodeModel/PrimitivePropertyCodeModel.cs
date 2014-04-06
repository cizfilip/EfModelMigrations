using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public abstract class PrimitivePropertyCodeModel : PropertyCodeModel
    {
        public bool IsTypeNullable { get; protected set; }

        public bool? IsRequired { get; set; }
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public int? ColumnOrder { get; set; }
        public IList<Tuple<string, object>> ColumnAnnotations { get; set; }
        public DatabaseGeneratedOption? DatabaseGeneratedOption { get; set; }
        public bool? IsConcurrencyToken { get; set; }
        public string ParameterName { get; set; }
        public int? MaxLength { get; set; }
        


        //TODO: ParameterName, ColumnOrder, ConcurrencyToken, ColumnAnnotation a dalsi - vse co lze mapovat pomoci fluent api
        // Pokud pridam nove property musim dodelat implementace MergeWithNullability v podtypech

        public PrimitivePropertyCodeModel(string name)
            : base(name)
        {
            this.IsTypeNullable = false;
        }

        public abstract PrimitivePropertyCodeModel MergeWith(PropertyCodeModel property, bool? newNullability = null);
        

        public static bool TryUnwrapNullability(string type, out string underlayingType)
        {
            Check.NotEmpty(type, "type");

            string lowerType = type.Trim().ToLowerInvariant();
            if (lowerType.EndsWith("?"))
            {
                underlayingType = lowerType.TrimEnd(new char[] { '?' });
                return true;
            }

            var match = Regex.Match(lowerType, NullableTypeRegEx, RegexOptions.None);
            if (match.Success)
            {
                underlayingType = match.Groups["UnderlayingType"].Value;
                return true;
            }

            underlayingType = type;
            return false;
        }

        private static readonly string NullableTypeRegEx = "^(system.)?nullable<(?<UnderlayingType>[^>]+)>$";
    }
}
