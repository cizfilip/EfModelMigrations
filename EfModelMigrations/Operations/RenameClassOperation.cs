using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public class RenameClassOperation : IModelChangeOperation
    {
        public string OldName { get; private set; }
        public string NewName { get; private set; }
        
        public RenameClassOperation(string oldName, string newName)
        {
            Check.NotEmpty(oldName, "oldName");
            Check.NotEmpty(newName, "newName");

            this.OldName = oldName;
            this.NewName = newName;
        }
       
    }
}
