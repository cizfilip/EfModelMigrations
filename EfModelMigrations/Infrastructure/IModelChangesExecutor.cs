using EfModelMigrations.Operations;
using System.Collections.Generic;

namespace EfModelMigrations.Infrastructure
{
    public interface IModelChangesExecutor
    {
        void Execute(IEnumerable<IModelChangeOperation> operations);
    }
}
