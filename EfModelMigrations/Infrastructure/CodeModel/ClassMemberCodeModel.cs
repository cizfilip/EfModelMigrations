using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public abstract class ClassMemberCodeModel
    {
        public ClassMemberCodeModel()
        {
            this.Name = null;
            this.Visibility = CodeModelVisibility.Public;
            this.IsSetterPrivate = false;
        }

        public string Name { get; set; }
        public CodeModelVisibility Visibility { get; set; }
        public bool IsSetterPrivate { get; set; }
    }
}
