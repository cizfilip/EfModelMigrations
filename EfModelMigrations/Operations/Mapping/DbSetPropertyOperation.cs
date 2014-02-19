using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public abstract class DbSetPropertyOperation : IModelChangeOperation
    {
        public string ClassName { get; private set; }

        public DbSetPropertyOperation(string className)
        {
            this.ClassName = className;
        }
    }
}
