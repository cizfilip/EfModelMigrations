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
        public string Name { get; set; }
        public string Type { get; set; }
        public string Visibility { get; set; }
        public virtual bool IsSetterPrivate { get; set; }
        public virtual bool IsVirtual { get; set; }
    }
}
