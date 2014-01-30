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
    public class RemoveClassTransformation : TransformationWithInverse
    {
        public string Name { get; private set; }
            
        public RemoveClassTransformation(string name, ModelTransformation inverse) : base(inverse)
        {
            this.Name = name;
        }

        public RemoveClassTransformation(string name)
            : this(name, null)
        {
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new RemoveClassOperation(Name);
        }

        public override IEnumerable<IMappingInformation> GetMappingInformationsToRemove(IClassModelProvider modelProvider)
        {
            yield return new DbSetPropertyInfo(Name);
        }

        //TODO: pri dropu tabulky ci sloupecku se musi kontrolovat zda-li se s tim mazou nejaka data - a kdyby ano
        //tak operaci provést jenom pokud byl predan parameter -Force
        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.DropTableOperation(Name);
        }
    }
}
