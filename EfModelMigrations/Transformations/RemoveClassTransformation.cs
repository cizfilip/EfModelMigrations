using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Operations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class RemoveClassTransformation : ModelTransformation
    {
        private ClassCodeModel classModel;
        public ClassCodeModel ClassModel
        {
            get
            {
                return classModel;
            }
        }

        public RemoveClassTransformation(ClassCodeModel classModel)
        {
            this.classModel = classModel;
        }
        
        public override IEnumerable<ModelChangeOperation> GetModelChangeOperations()
        {
            foreach (var property in classModel.Properties)
            {
                yield return new RemovePropertyFromClassOperation(classModel, property);
            }
            yield return new RemoveClassOperation(classModel);
        }

        public override ModelTransformation Inverse()
        {
            return new CreateClassTransformation(classModel);
        }

        public override MigrationOperation GetDbMigrationOperation()
        {
            throw new NotImplementedException();
        }
    }
}
