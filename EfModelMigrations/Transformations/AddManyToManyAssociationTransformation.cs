using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class AddManyToManyAssociationTransformation : AddAssociationTransformation
    {
        public ManyToManyJoinTable JoinTable { get; private set; }

        public AddManyToManyAssociationTransformation(AssociationMemberInfo principal, AssociationMemberInfo dependent, ManyToManyJoinTable joinTable)
            :base(principal, dependent)
        {
            this.JoinTable = joinTable;
        }


        protected override AssociationInfo CreateMappingInformation()
        {
            return new ManyToManyAssociationInfo(Principal, Dependent, JoinTable);
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            return builder.ManyToManyRelationOperations(Principal.ClassName, Dependent.ClassName, JoinTable.TableName, JoinTable.PrincipalForeignKeyColumns, JoinTable.DependentForeignKeyColumns);
        }

        public override ModelTransformation Inverse()
        {
            return null;
        }
    }
}
