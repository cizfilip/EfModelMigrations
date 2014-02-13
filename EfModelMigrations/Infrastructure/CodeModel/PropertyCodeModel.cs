using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public class PropertyCodeModel : ClassMemberCodeModel
    {
        public PropertyCodeModel() 
            :base()
        {
            Type = null;
        }

        public string Type { get; set; }
                
    }
}
