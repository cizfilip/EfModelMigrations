using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public abstract class PropertyCodeModel
    {
        public virtual string Name { get; set; }
        public virtual CodeModelVisibility? Visibility { get; set; }
        public virtual bool? IsSetterPrivate { get; set; }
        public virtual bool? IsVirtual { get; set; }
        
        
        internal PropertyCodeModel()
        {
            this.Name = null;
        }

        public PropertyCodeModel(string name)
        {
            this.Name = name;
        }
    }
}
