using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class AddDbSetPropertyOperation : DbSetPropertyOperation
    {        
        public AddDbSetPropertyOperation(string className)
            : base(className)
        {
        }
    }
}
