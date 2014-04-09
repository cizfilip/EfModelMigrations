using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class RemoveClassMapping : IRemoveMappingInformation
    {
        public string Name { get; private set; }

        public RemoveClassMapping(string className)
        {
            Check.NotEmpty(className, "className");

            this.Name = className;
        }

    }
}
