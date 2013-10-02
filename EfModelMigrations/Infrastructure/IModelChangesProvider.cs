using EfModelMigrations.Infrastructure.CodeModel;

namespace EfModelMigrations.Infrastructure
{
    public interface IModelChangesProvider
    {
        IDbContextChangesProvider ChangeDbContext { get; }


        void CreateEmptyClass(ClassCodeModel classModel);
        void RemoveClass(ClassCodeModel classModel);

        void AddPropertyToClass(ClassCodeModel classModel, PropertyCodeModel propertyModel);
        void RemovePropertyFromClass(ClassCodeModel classModel, PropertyCodeModel propertyModel);
    }
}
