using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class AddOneToOneForeignKeyAssociationTransformation : AddOneToOneAssociationTransformation
    {   
        public string[] ForeignKeyColumnNames { get; private set; }

        public AddOneToOneForeignKeyAssociationTransformation(AssociationMemberInfo principal, AssociationMemberInfo dependent, OneToOneAssociationType type, string[] foreignKeyColumnNames, bool willCascadeOnDelete)
            :base(principal, dependent, type, willCascadeOnDelete)
        {
            this.ForeignKeyColumnNames = foreignKeyColumnNames;
        }

        protected override AssociationInfo CreateMappingInformation()
        {
            return new OneToOneWithForeignKeysAssociationInfo(Principal, Dependent, Type, ForeignKeyColumnNames, WillCascadeOnDelete);
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            bool isDependentRequired = Type == OneToOneAssociationType.BothEndsOptional ? false : true;

            return builder.OneToOneForeignKeyRelationOperations(Principal.ClassName, Dependent.ClassName, isDependentRequired, ForeignKeyColumnNames, WillCascadeOnDelete);
        }

        public override ModelTransformation Inverse()
        {
            return null;
        }
    }
}
