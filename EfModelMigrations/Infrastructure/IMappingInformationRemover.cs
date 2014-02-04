using EfModelMigrations.Operations.Mapping;

namespace EfModelMigrations.Infrastructure
{
    public interface IMappingInformationRemover
    {
        void Remove(IMappingInformation mappingInformation);
    }
}
