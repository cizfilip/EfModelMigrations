using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public class ScalarProperty : PropertyCodeModel<ScalarType>
    {
        public ScalarProperty(ScalarType type)
            : base(type)
        {
        }

        public ScalarProperty(string name, ScalarType type)
            : base(name, type)
        {
        }  
    }
}
