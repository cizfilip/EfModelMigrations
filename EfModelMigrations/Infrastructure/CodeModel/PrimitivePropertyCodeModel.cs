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
        public ColumnInfo Column { get; protected set; }

        
        public PrimitivePropertyCodeModel(string name)
            : base(name)
        {
            this.IsTypeNullable = false;
            this.Column = new ColumnInfo();
        }

        protected abstract PrimitivePropertyCodeModel CreateForMerge(PropertyCodeModel property, bool? newNullability = null);
        
        public PrimitivePropertyCodeModel MergeWith(PropertyCodeModel property, bool? newNullability = null)
        {
            var merged = CreateForMerge(property, newNullability);
            
            merged.IsVirtual = property.IsVirtual;
            merged.IsSetterPrivate = property.IsSetterPrivate;
            merged.Visibility = property.Visibility;
            merged.Column = this.Column.Copy();

            return merged;
        }
        

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
