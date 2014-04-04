using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class RemovePropertyMapping : IRemoveMappingInformation
    {
        public string ClassName { get; private set; }
        public string PropertyName { get; private set; }

        public RemovePropertyMapping(string className, string propertyName)
        {
            Check.NotEmpty(className, "className");
            Check.NotEmpty(propertyName, "propertyName");

            this.ClassName = className;
            this.PropertyName = propertyName;
        }

    }
}
