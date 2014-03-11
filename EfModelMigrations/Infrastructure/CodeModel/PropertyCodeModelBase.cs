using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public abstract class PropertyCodeModelBase
    {
        public PropertyCodeModelBase()
        {
            this.Name = null;
            this.Visibility = CodeModelVisibility.Public;
            this.IsSetterPrivate = false;
            this.IsVirtual = false;
        }

        public string Name { get; set; }
        public CodeModelVisibility Visibility { get; set; }
        //TODO: moznost privatniho setteru mozna zrusit - pokud se rozhodnu zachovat -> dodelat property buildery
        public bool IsSetterPrivate { get; set; }

        public bool IsVirtual { get; set; }


        public abstract CodeModelType GetType();
    }
}
