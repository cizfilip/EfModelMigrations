using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class RemoveMappingInformationOperation : MappingInformationOperation<IRemoveMappingInformation>
    {
        public RemoveMappingInformationOperation(IRemoveMappingInformation mappingInformation)
            : base(mappingInformation) { }
    }
}
