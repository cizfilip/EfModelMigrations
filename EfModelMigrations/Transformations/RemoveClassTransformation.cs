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
            Check.NotEmpty(name, "name");

            this.Name = name;
        }

        public RemoveClassTransformation(string name)
            : this(name, null)
        {
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            //TODO: a co mapovaci informace jednak o tride a jednak o jejich property??
            yield return new RemoveMappingInformationOperation(new RemoveClassMapping(Name));
            yield return new RemoveClassOperation(Name);

            yield return new RemoveDbSetPropertyOperation(Name);
        }
        
        //TODO: pri dropu tabulky ci sloupecku se musi kontrolovat zda-li se s tim mazou nejaka data - a kdyby ano
        //tak operaci provést jenom pokud byl predan parameter -Force
        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            yield return builder.DropTableOperation(
                    builder.OldModel.GetStoreEntitySetForClass(Name)
                );
        }

        public override bool IsDestructiveChange
        {
            get { return true; }
        }
    }
}
