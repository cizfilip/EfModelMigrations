using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
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
    class ExtractClassTransformation : ModelTransformation
    {
        public string FromClass { get; private set; }

        public string[] Properties { get; private set; }

        public string NewClass { get; private set; }


        public ExtractClassTransformation(string fromClass, string[] properties, string newClass)
        {
            this.FromClass = fromClass;
            this.Properties = properties;
            this.NewClass = newClass;
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new CreateEmptyClassOperation(NewClass);

            yield return new AddPropertyToClassOperation(NewClass, new ScalarProperty("Id", new ScalarType(PrimitiveTypeKind.Int32)));
            
            foreach (var prop in Properties)
            {
                yield return new MovePropertyOperation(FromClass, NewClass, prop);
            }

            var associationOperations = GetAssociationTransformation().GetModelChangeOperations(modelProvider);
            foreach (var assocOp in associationOperations)
            {
                yield return assocOp;
            }
        }


        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            return base.GetDbMigrationOperations(builder);
        }


        public override ModelTransformation Inverse()
        {
            return null;
        }


        private AddOneToOneForeignKeyAssociationTransformation GetAssociationTransformation()
        {
            throw new NotImplementedException();

            //var principal = new AssociationMemberInfo(FromClass);

            //var dependent = new AssociationMemberInfo(FromClass);

            //return new AddOneToOneForeignKeyAssociationTransformation(principal, dependent, OneToOneAssociationType.BothEndsRequired, ,);
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
