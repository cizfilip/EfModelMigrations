using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Operations;
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
            yield return new CreateClassOperation(classModel);
            foreach (var property in classModel.Properties)
            {
                yield return new AddPropertyToClassOperation(classModel, property);
            }
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveClassTransformation(classModel);
        }

        public override MigrationOperation GetDbMigrationOperation()
        {
            throw new NotImplementedException();
        }
    }
}
