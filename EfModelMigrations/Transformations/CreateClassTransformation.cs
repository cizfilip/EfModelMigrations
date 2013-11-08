using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.DbContext;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class CreateClassTransformation : ModelTransformation
    {
        public string ClassName { get; private set; }
        public IEnumerable<PropertyCodeModel> Properties { get; private set; }

        //TODO: Pridat ve vsech transformaci validace na parametry v konstruktoru - jako v commandech
        public CreateClassTransformation(string className, IEnumerable<PropertyCodeModel> properties)
        {
            this.ClassName = className;
            this.Properties = properties;
        }

        public override IEnumerable<ModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            var classModel = modelProvider.CreateClassCodeModel(ClassName, null, null, null, Properties);

            //TODO: vyhayovat vyjimky pokud trida jiz existuje... i jinde treba v addproperty pokud property jiz existuje atd..
            yield return new CreateClassOperation(classModel);
            foreach (var property in Properties)
            {
                yield return new AddPropertyToClassOperation(classModel, property);
            }
            yield return new AddDbSetPropertyOperation(classModel);
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveClassTransformation(ClassName);
        }

        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.CreateTableOperation(ClassName);
        }
    }
}
