using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class AddManyToManyAssociationTransformation : AddAssociationTransformation
    {
        public ManyToManyJoinTable JoinTable { get; private set; }

        public AddManyToManyAssociationTransformation(AssociationMemberInfo source, AssociationMemberInfo target, ManyToManyJoinTable joinTable)
            : base(source, target)
        {
            this.JoinTable = joinTable;

            //TODO: stringy do resourců
            if (!(source.Multipticity == RelationshipMultiplicity.Many && target.Multipticity == RelationshipMultiplicity.Many))
            {
                throw new ModelTransformationValidationException("Invalid association multiplicity for many to many association.");
            }
        }


        protected override AddAssociationMapping CreateMappingInformation()
        {
            return new AddAssociationMapping(Principal, Dependent)
                {
                    JoinTable = JoinTable
                };
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            return builder.ManyToManyRelationOperations(Principal.ClassName, Dependent.ClassName, JoinTable.TableName, JoinTable.SourceForeignKeyColumns, JoinTable.TargetForeignKeyColumns);
        }

        public override ModelTransformation Inverse()
        {
            return null;
        }
    }
}
