using EfModelMigrations.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public abstract class ModelChangeOperation
    {
        public abstract void ExecuteModelChanges(IModelChangesProvider provider);

        public abstract ModelChangeOperation Inverse();
    }
}
