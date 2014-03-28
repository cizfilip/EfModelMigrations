using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public abstract class MappingInformationOperation<MapInfo> : IModelChangeOperation where MapInfo : IMappingInformation
    {
        public MapInfo MappingInformation { get; protected set; }

        public MappingInformationOperation(MapInfo mappingInformation)
        {
            this.MappingInformation = mappingInformation;
        }
    }
}
