using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Resources;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    //TODO: pro many to many zda se neni zpusob jak explicitne namapovat ze fk v join table budou mit indexy - tudiz spoleham na konvence....
    public class AddManyToManyAssociationTransformation : AddAssociationTransformation
    {
        public AddManyToManyAssociationTransformation(AssociationCodeModel model)
            : base(model)
        {
            if (!Model.IsOneToMany())
            {
                throw new ModelTransformationValidationException(Strings.Transformations_InvalidMultiplicityManyToMany);
            }
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            //add join table
            var joinTableEntitySet = builder.NewModel.GetStoreEntitySetJoinTableForManyToMany(Model.Principal, Model.Dependent);
            yield return builder.CreateTableOperation(joinTableEntitySet);

            //add foreign keys and indexes
            var joinTableRelations = joinTableEntitySet.EntityContainer.AssociationSets
                .Where(a => a.AssociationSetEnds.ElementAt(1).EntitySet == joinTableEntitySet);

            foreach (var relation in joinTableRelations)
            {
                var referentialConstraint = relation.ElementType.Constraint;

                var indexOperation = builder.TryBuildCreateIndexOperation(joinTableEntitySet, referentialConstraint.ToProperties);
                if (indexOperation != null)
                {
                    yield return indexOperation;
                }
                yield return builder.AddForeignKeyOperation(referentialConstraint);
            }
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveManyToManyAssociationTransformation(Model.Principal.ToSimpleAssociationEnd(), Model.Dependent.ToSimpleAssociationEnd());
        }
    }
}
