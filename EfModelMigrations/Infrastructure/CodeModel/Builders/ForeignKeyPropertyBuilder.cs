using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel.Builders
{
    public interface IForeignKeyPropertyBuilder : IFluentInterface
    {
        ForeignKeyPropertyCodeModel Build(
            string columnName = null,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null);
    }

    public sealed class ForeignKeyPropertyBuilder : IForeignKeyPropertyBuilder
    {
        public ForeignKeyPropertyCodeModel Build(
            string columnName = null,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new ForeignKeyPropertyCodeModel()
                {
                    ColumnName = columnName,
                    Visibility = visibility,
                    IsVirtual = isVirtual,
                    IsSetterPrivate = isSetterPrivate
                };
        }
    }
}
