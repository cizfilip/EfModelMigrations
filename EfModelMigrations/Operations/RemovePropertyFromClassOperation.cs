using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public class RemovePropertyFromClassOperation : IModelChangeOperation
    {
        public string ClassName { get; private set; }
        public string Name { get; private set; }
        
        public RemovePropertyFromClassOperation(string className, string name)
        {
            this.ClassName = className;
            this.Name = name;
        }

    }
}
