using EfModelMigrations.Operations.Mapping.Model;

namespace EfModelMigrations.Operations.Mapping
{
    public interface IAddMappingInformation : IMappingInformation
    {
        EfFluentApiCallChain BuildEfFluentApiCallChain();

        //TODO: umoznit delat i mapovani pomoci atributu
        //IEnumerable<AttributeInfo> BuildEfMappingAttributes();
    }
}
