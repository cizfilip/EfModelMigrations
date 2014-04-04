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

        public ScalarPropertyCodeModel MergeWithScalarProperty(ScalarPropertyCodeModel property)
        {
            var merged = new ScalarPropertyCodeModel(Name, property.ColumnModel);

            merged.IsSetterPrivate = IsSetterPrivate;
            merged.IsVirtual = IsVirtual;
            merged.Visibility = Visibility;

            return merged;
        }
    }
}
