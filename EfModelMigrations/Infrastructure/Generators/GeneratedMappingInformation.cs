using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    public class GeneratedMappingInformation
    {
        public MappingInformationType Type { get; set; }

        public string Value { get; set; }
    }

    public enum MappingInformationType
    {
        DbContextProperty,
        EntityTypeConfiguration,
        CodeAttribute
    }
}
