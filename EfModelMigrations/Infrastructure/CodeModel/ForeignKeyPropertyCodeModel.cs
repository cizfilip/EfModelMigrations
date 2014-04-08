using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public sealed class ForeignKeyPropertyCodeModel : PropertyCodeModel
    {
        public string ColumnName { get; set; }

        public ForeignKeyPropertyCodeModel(string name)
            :base(name)
        {
        }

        internal ForeignKeyPropertyCodeModel()
            :this(null)
        {
        }

    }
}
