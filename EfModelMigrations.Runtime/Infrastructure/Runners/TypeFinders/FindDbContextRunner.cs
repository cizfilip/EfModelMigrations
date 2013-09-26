using EfModelMigrations.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.TypeFinders
{
    
    [Serializable]
    internal class FindDbContextRunner : BaseRunner
    {
        public override void Run()
        {
            var typeFinder = new TypeFinder();

            bool dbConfigurationExists = false;
            Type foundType;

            if (typeFinder.TryFindDbContextType(ProjectAssembly, out foundType))
            {
                dbConfigurationExists = true;
            }


            Return(dbConfigurationExists);
        }
    }
}
