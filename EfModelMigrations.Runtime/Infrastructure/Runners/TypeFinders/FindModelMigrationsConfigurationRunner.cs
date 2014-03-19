using EfModelMigrations.Configuration;
using EfModelMigrations.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.TypeFinders
{
    [Serializable]
    internal class FindModelMigrationsConfigurationRunner : BaseRunner
    {
        public override void Run()
        {
            var typeFinder = new TypeFinder();

            Type foundType;

            if (typeFinder.TryFindModelMigrationsConfigurationType(ProjectAssembly, out foundType))
            {
                Return(foundType.FullName);
            }


            Return((string)null);
        }
    }
}
