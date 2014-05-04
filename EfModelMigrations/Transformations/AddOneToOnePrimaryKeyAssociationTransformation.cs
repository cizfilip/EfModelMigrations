using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Transformations.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using EfModelMigrations.Resources;
using EfModelMigrations.Transformations.Preconditions;

namespace EfModelMigrations.Transformations
{
    public class AddOneToOnePrimaryKeyAssociationTransformation : AddAssociationTransformation
    {
        public AddOneToOnePrimaryKeyAssociationTransformation(AssociationCodeModel model)
            :base(model)
        {
            if (Model.IsOneToOne() && !(Model.Principal.Multipticity == RelationshipMultiplicity.ZeroOrOne && Model.Dependent.Multipticity == RelationshipMultiplicity.ZeroOrOne))
            {
                throw new ModelTransformationValidationException(Strings.Transformations_InvalidMultiplicityOneToOnePk);
            }
        }

        public override IEnumerable<ModelTransformationPrecondition> GetPreconditions()
        {
            var basePreconditions = base.GetPreconditions().ToList();
            basePreconditions.Add(new ClassesHaveSamePrimaryKeyPrecondition(new[] { Model.Principal.ClassName, Model.Dependent.ClassName }));

            return basePreconditions;
        }


        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            var dependentStoreEntitySet = builder.NewModel.GetStoreEntitySetForClass(Model.Dependent.ClassName);

            //drop identity if required
            var dropIdentityOperation = builder.TryBuildDropIdentityOperation(dependentStoreEntitySet);
            if(dropIdentityOperation != null)
            {
                yield return dropIdentityOperation;
            }

            var referentialConstraint = builder.NewModel.GetStoreAssociationTypeForAssociation(Model.Principal, Model.Dependent)
                .Constraint;
            var foreignKeyColumns = referentialConstraint.ToProperties;

            //add index on Fk if exists on ef model
            var indexOperation = builder.TryBuildCreateIndexOperation(dependentStoreEntitySet, foreignKeyColumns);
            if (indexOperation != null)
            {
                yield return indexOperation;
            }

            //add foreign key
            yield return builder.AddForeignKeyOperation(referentialConstraint);
        }


        public override ModelTransformation Inverse()
        {
            return new RemoveOneToOnePrimaryKeyAssociationTransformation(Model.Principal.ToSimpleAssociationEnd(), Model.Dependent.ToSimpleAssociationEnd(), true);
        }  
    }
}
