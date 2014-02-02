using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public abstract class MappingInformationOperation : IModelChangeOperation
    {
        public IMappingInformation MappingInformation { get; protected set; }

        public MappingInformationOperation(IMappingInformation mappingInformation)
        {
            this.MappingInformation = mappingInformation;
        }
    }
}
