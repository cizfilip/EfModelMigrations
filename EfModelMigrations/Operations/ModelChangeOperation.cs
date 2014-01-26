using EfModelMigrations.Infrastructure;

namespace EfModelMigrations.Operations
{
    public abstract class ModelChangeOperation
    {
        //TODO: předělat vazbu mezi IModelChangesProvider a IDbContextChangesProvider a místo toho zde předávat nějaký context object
        public abstract void ExecuteModelChanges(IModelChangesProvider provider);

        public abstract ModelChangeOperation Inverse();
    }
}
