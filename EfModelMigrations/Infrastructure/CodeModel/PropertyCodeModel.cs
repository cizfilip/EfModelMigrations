using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public abstract class PropertyCodeModel
    {
        private string name;
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                Check.NotEmpty(value, "Name");
                name = value;
            }
        }
        public virtual CodeModelVisibility? Visibility { get; set; }
        public virtual bool? IsSetterPrivate { get; set; }
        public virtual bool? IsVirtual { get; set; }
        
        public PropertyCodeModel(string name)
        {
            this.Name = name;
        }
    }
}
