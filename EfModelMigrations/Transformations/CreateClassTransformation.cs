using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using EfModelMigrations.Operations.Mapping;

namespace EfModelMigrations.Transformations
{
    public class CreateClassTransformation : ModelTransformation
    {
        public string Name { get; private set; }
        public IEnumerable<ScalarProperty> Properties { get; private set; }

        //TODO: Pridat ve vsech transformaci validace na parametry v konstruktoru - jako v commandech
        public CreateClassTransformation(string name, IEnumerable<ScalarProperty> properties)
        {
            this.Name = name;
            this.Properties = properties;
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            //TODO: vyhazovat vyjimky pokud trida jiz existuje... i jinde treba v addproperty pokud property jiz existuje atd..
            yield return new CreateEmptyClassOperation(Name);
            foreach (var property in Properties)
            {
                yield return new AddPropertyToClassOperation(Name, property);
            }

            yield return new AddDbSetPropertyOperation(Name);
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            yield return builder.CreateTableOperation(Name);
        }
        
        public override ModelTransformation Inverse()
        {
            return new RemoveClassTransformation(Name, this);
        }

    }
}
