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

        protected override PrimitivePropertyCodeModel CreateForMerge(PropertyCodeModel property, bool? newNullability = null)
        {
            return new EnumPropertyCodeModel(property.Name, EnumType,
                newNullability.HasValue ? newNullability.Value : this.IsTypeNullable);
        }
    }
}
