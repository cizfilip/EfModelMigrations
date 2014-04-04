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

        public EnumPropertyCodeModel(string name, string enumTypeName)
            :base(name)
        {
            this.EnumType = enumTypeName;
        }

        internal EnumPropertyCodeModel(string enumTypeName)
            :this(null, enumTypeName)
        {

        }
        

        //TODO: dodelat copy vsech vlastnosti
        public override PrimitivePropertyCodeModel Copy()
        {
            return new EnumPropertyCodeModel(Name, EnumType)
            {
                ColumnName = ColumnName,
                ColumnType = ColumnType,
                HasDatabaseGeneratedOption = HasDatabaseGeneratedOption,
                IsRequired = IsRequired,
                IsSetterPrivate = IsSetterPrivate,
                IsVirtual = IsVirtual,
                Visibility = Visibility
            };
        }
    }
}
