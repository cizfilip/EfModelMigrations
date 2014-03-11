using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators.Templates
{
    internal partial class PropertyTemplate
    {
        public PropertyCodeModelBase PropertyModel { get; set; }
        public Func<CodeModelVisibility, string> CodeModelVisibilityMapper { get; set; }

        public Func<CodeModelType, string> CodeModelTypeMapper { get; set; }
    }
}
