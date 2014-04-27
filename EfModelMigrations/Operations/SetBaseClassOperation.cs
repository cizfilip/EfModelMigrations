using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public class SetBaseClassOperation : IModelChangeOperation
    {
        public string ForClass { get; private set; }
        public string BaseClass { get; private set; }

        public SetBaseClassOperation(string forClass, string baseClass)
        {
            Check.NotEmpty(forClass, "forClass");
            Check.NotEmpty(baseClass, "baseClass");

            this.ForClass = forClass;
            this.BaseClass = baseClass;
        }
    }
}
