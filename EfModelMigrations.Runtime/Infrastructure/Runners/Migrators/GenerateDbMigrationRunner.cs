using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.Migrators
{
    [Serializable]
    internal class GenerateDbMigrationRunner : MigratorBaseRunner
    {
        public bool IsRevert { get; set; }


        public override void Run()
        {
            IEnumerable<ModelTransformation> transformations = GetModelTransformations(IsRevert);

        }
    }
}
