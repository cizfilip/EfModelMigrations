using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class RemoveAssociationMapping : IRemoveMappingInformation
    {
        public SimpleAssociationEnd Source { get; private set; }
        public SimpleAssociationEnd Target { get; private set; }

        public RemoveAssociationMapping(SimpleAssociationEnd source, SimpleAssociationEnd target)
        {
            Check.NotNull(source, "source");
            Check.NotNull(target, "target");

            this.Source = source;
            this.Target = target;
        }
        
    }
}
