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
        public ClassCodeModel ClassModel { get; private set; }


        public CreateClassTransformation(ClassCodeModel classModel)
        {
            this.ClassModel = classModel;
        }

        public override IEnumerable<ModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            //TODO: vyhayovat vyjimky pokud trida jiz existuje... i jinde treba v addproperty pokud property jiz existuje atd..
            yield return new CreateClassOperation(ClassModel);
            foreach (var property in ClassModel.Properties)
            {
                yield return new AddPropertyToClassOperation(ClassModel, property);
            }
            yield return new AddDbSetPropertyOperation(ClassModel);
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveClassTransformation(ClassModel.Name);
        }

        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.CreateTableOperation(ClassModel);
        }
    }
}
