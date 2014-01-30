using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public class RenamePropertyOperation : IModelChangeOperation
    {
        public string ClassName { get; private set; }
        public string OldName { get; private set; }
        public string NewName { get; private set; }
        
        public RenamePropertyOperation(string className, string oldName, string newName)
        {
            this.ClassName = className;
            this.OldName = oldName;
            this.NewName = newName;
        }

    }
}
