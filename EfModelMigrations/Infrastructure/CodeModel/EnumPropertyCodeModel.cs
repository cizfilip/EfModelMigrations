using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public sealed class EnumPropertyCodeModel : PrimitivePropertyCodeModel
    {
        public string EnumType { get; private set; }

        public EnumPropertyCodeModel(string name, string enumTypeName, bool isTypeNullable)
            :base(name)
        {
            this.EnumType = enumTypeName;
            this.IsTypeNullable = isTypeNullable;
        }

        internal EnumPropertyCodeModel(string enumTypeName, bool isTypeNullable)
            :this(null, enumTypeName, isTypeNullable)
        {
        }

        public override PrimitivePropertyCodeModel MergeWith(PropertyCodeModel property, bool? newNullability = null)
        {
            return new EnumPropertyCodeModel(property.Name, EnumType, 
                newNullability.HasValue ? newNullability.Value : this.IsTypeNullable)
            {
                IsVirtual = property.IsVirtual,
                IsSetterPrivate = property.IsSetterPrivate,
                Visibility = property.Visibility,

                ColumnName = this.ColumnName,
                ColumnType = this.ColumnType,
                DatabaseGeneratedOption = this.DatabaseGeneratedOption,
                IsRequired = this.IsRequired,
            };
        }
    }
}
