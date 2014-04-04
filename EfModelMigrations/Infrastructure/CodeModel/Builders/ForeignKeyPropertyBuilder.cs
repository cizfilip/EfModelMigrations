using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel.Builders
{
    public sealed class ForeignKeyPropertyBuilder : IFluentInterface
    {
        public ForeignKeyPropertyCodeModel Build(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new ForeignKeyPropertyCodeModel()
                {
                    Visibility = visibility,
                    IsVirtual = isVirtual,
                    IsSetterPrivate = isSetterPrivate
                };
        }
    }
}
