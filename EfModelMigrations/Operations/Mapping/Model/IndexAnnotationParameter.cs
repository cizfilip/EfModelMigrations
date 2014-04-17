using System.ComponentModel.DataAnnotations.Schema;

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
