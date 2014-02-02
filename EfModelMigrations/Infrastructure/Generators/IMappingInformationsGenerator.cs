using EfModelMigrations.Operations.Mapping;

namespace EfModelMigrations.Infrastructure.Generators
{
    public interface IMappingInformationsGenerator
    {
        GeneratedMappingInformation Generate(IMappingInformation mappingInformation);
    }
}
