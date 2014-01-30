using EfModelMigrations.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure
{
    public interface IMappingInformationsProvider
    {
        void AddMappingInformations(IEnumerable<IMappingInformation> mappingInformations);

        void RemoveMappingInformations(IEnumerable<IMappingInformation> mappingInformations);
    }
}
