using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public class PropertyCodeModel
    {

        public PropertyCodeModel()
        {
            Name = null;
            Type = null;
            Visibility = CodeModelVisibility.Public;
            IsSetterPrivate = false;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public CodeModelVisibility Visibility { get; set; }
        public bool IsSetterPrivate { get; set; }
                
    }
}
