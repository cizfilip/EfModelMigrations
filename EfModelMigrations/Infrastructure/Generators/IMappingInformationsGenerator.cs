using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Operations.Mapping.Model;

namespace EfModelMigrations.Infrastructure.Generators
{
    public interface IMappingInformationsGenerator
    {
        GeneratedFluetApiCall GenerateFluentApiCall(EfFluentApiCallChain callChain);

        string GetPrefixForOnModelCreatingUse(string entityName);

        //TODO: umoznit delat i mapovani pomoci atributu
        //IEnumerable<GeneratedAttribute> BuildEfMappingAttributes(IEnumerable<AttributeInfo> attributes);
    }
}
