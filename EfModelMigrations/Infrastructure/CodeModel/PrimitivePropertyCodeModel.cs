using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public abstract class PrimitivePropertyCodeModel : PropertyCodeModel
    {
        public string ColumnName { get; set; }
        public bool? IsRequired { get; set; }
        public DatabaseGeneratedOption? HasDatabaseGeneratedOption { get; set; }
        public string ColumnType { get; set; }

        //TODO: ParameterName, ColumnOrder, ConcurrencyToken, ColumnAnnotation a dalsi - vse co lze mapovat pomoci fluent api

        public PrimitivePropertyCodeModel(string name)
            :base(name)
        {
        }

        public abstract PrimitivePropertyCodeModel Copy();
    }
}
