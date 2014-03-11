using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public class AddPropertyToClassOperation : IModelChangeOperation
    {
        public string ClassName { get; private set; }
        public PropertyCodeModelBase Model { get; private set; }

        public AddPropertyToClassOperation(string className, PropertyCodeModelBase model)
        {
            this.ClassName = className;
            this.Model = model;
        }
        
    }
}
