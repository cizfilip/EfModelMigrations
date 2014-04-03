using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class RemovePropertyMapping : IRemoveMappingInformation
    {
        public string PropertyName { get; private set; }

        public RemovePropertyMapping(string propertyName)
        {
            Check.NotEmpty(propertyName, "propertyName");

            this.PropertyName = propertyName;
        }

    }
}
