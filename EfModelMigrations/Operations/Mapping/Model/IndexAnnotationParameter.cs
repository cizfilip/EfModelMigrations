using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping.Model
{
    public class IndexAnnotationParameter : IEfFluentApiMethodParameter
    {
        public IndexAttribute[] Indexes { get; private set; }

        public IndexAnnotationParameter(IndexAttribute index)
            : this(new[] { index })
        {
        }

        public IndexAnnotationParameter(IndexAttribute[] indexes)
        {
            Check.NotNull(indexes, "indexes");

            this.Indexes = indexes;
        }
    }
}
