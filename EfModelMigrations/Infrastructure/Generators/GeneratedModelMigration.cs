using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    [Serializable]
    public class GeneratedModelMigration
    {
        public string MigrationDirectory { get; set; }
        public string MigrationId { get; set; }
        public string SourceCode { get; set; }
    }
}
