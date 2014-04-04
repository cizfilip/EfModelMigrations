using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public sealed class ForeignKeyPropertyCodeModel : PropertyCodeModel
    {
        public ForeignKeyPropertyCodeModel(string name)
            :base(name)
        {
        }

        internal ForeignKeyPropertyCodeModel()
            :this(null)
        {
        }

        public PrimitivePropertyCodeModel MergeWithProperty(PrimitivePropertyCodeModel property)
        {
            var merged = property.Copy();

            merged.Name = Name;
            merged.IsSetterPrivate = IsSetterPrivate;
            merged.IsVirtual = IsVirtual;
            merged.Visibility = Visibility;

            return merged;
        }
    }
}
