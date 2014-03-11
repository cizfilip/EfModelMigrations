using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;

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

            yield return new RemoveDbSetPropertyOperation(Name);
        }
        
        //TODO: pri dropu tabulky ci sloupecku se musi kontrolovat zda-li se s tim mazou nejaka data - a kdyby ano
        //tak operaci provést jenom pokud byl predan parameter -Force
        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            yield return builder.DropTableOperation(Name);
        }

        public override bool IsDestructiveChange
        {
            get { return true; }
        }
    }
}
