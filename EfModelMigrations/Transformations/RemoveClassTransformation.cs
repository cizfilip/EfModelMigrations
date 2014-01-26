using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Mapping;
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
        private CreateClassTransformation inverse;

        public string ClassName { get; private set; }
            

        public RemoveClassTransformation(string className)
        {
            this.ClassName = className;
        }

        public RemoveClassTransformation(string className, CreateClassTransformation inverse) : this(className)
        {
            this.inverse = inverse;
        }

        public override IEnumerable<ModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            var classModel = modelProvider.GetClassCodeModel(ClassName);

            foreach (var property in classModel.Properties)
            {
                yield return new RemovePropertyFromClassOperation(classModel, property);
            }
            yield return new RemoveClassOperation(classModel);
        }

        public override IEnumerable<IMappingInformation> GetMappingInformations(IClassModelProvider modelProvider)
        {
            yield return new DbSetPropertyInfo(ClassName);
        }

        public override ModelTransformation Inverse()
        {
            return inverse;
        }

        //TODO: pri dropu tabulky ci sloupecku se musi kontrolovat zda-li se s tim mazou nejaka data - a kdyby ano
        //tak operaci provést jenom pokud byl predan parameter -Force
        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.DropTableOperation(ClassName);
        }
    }
}
