using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class CreateClassTransformation : ModelTransformation
    {
        public string Name { get; private set; }
        public IEnumerable<PropertyCodeModel> Properties { get; private set; }

        //TODO: Pridat ve vsech transformaci validace na parametry v konstruktoru - jako v commandech
        public CreateClassTransformation(string name, IEnumerable<PropertyCodeModel> properties)
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
            
        }

        public override IEnumerable<IMappingInformation> GetMappingInformationsToAdd(IClassModelProvider modelProvider)
        {
            yield return new DbSetPropertyInfo(Name);
        }

        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.CreateTableOperation(Name);
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveClassTransformation(Name, this);
        }

    }
}
