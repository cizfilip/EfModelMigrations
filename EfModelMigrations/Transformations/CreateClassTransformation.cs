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
        private ClassCodeModel classModel;
        public ClassCodeModel ClassModel
        {
            get
            {
                return classModel;
            }
        }


        public CreateClassTransformation(ClassCodeModel classModel)
        {
            this.classModel = classModel;
        }

        public override IEnumerable<ModelChangeOperation> GetModelChangeOperations()
        {
            //TODO: vyhayovat vyjimky pokud trida jiz existuje... i jinde treba v addproperty pokud property jiz existuje atd..
            yield return new CreateClassOperation(classModel);
            foreach (var property in classModel.Properties)
            {
                yield return new AddPropertyToClassOperation(classModel, property);
            }
            yield return new AddDbSetPropertyOperation(classModel);
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveClassTransformation(classModel);
        }

        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.CreateTableOperation(classModel);
        }
    }
}
