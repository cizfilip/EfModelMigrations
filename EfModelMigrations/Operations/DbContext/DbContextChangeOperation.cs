using EfModelMigrations.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.DbContext
{
    public abstract class DbContextChangeOperation : ModelChangeOperation
    {
        public override abstract void ExecuteModelChanges(IModelChangesProvider provider);

        public override abstract ModelChangeOperation Inverse();        
    }
}
