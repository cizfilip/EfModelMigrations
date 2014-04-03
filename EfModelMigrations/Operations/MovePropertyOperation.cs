using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    //TODO odstranit z projektu! az predelam extract 
    public class MovePropertyOperation : IModelChangeOperation
    {
        public string FromClassName { get; private set; }
        public string ToClassName { get; private set; }
        public string Name { get; private set; }

        public MovePropertyOperation(string fromClassName, string toClassName, string name)
        {
            this.FromClassName = fromClassName;
            this.ToClassName = toClassName;
            this.Name = name;
        }

    }
    
}
