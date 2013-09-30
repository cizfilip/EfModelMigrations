using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public abstract class ModelChangeOperation
    {
        public abstract void ExecuteModelChanges();

        public abstract ModelChangeOperation Inverse();
    }
}
