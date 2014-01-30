using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public class RemoveClassOperation : IModelChangeOperation
    {
        public string Name { get; private set; }

        public RemoveClassOperation(string name)
        {
            this.Name = name;
        }
        
    }
}
