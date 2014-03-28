using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.CodeModel.Builders;
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
    public class ExtractClassTransformation : ModelTransformation
    {
        public string FromClass { get; private set; }

        public string[] Properties { get; private set; }

        public string NewClass { get; private set; }

        public string[] ForeignKeyColumns { get; private set; }


        public ExtractClassTransformation(string fromClass, string[] properties, string newClass, string[] foreignKeyColumns)
        {
            this.FromClass = fromClass;
            this.Properties = properties;
            this.NewClass = newClass;
            this.ForeignKeyColumns = foreignKeyColumns;
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new CreateEmptyClassOperation(NewClass);

            yield return new AddPropertyToClassOperation(NewClass, GetPrimaryKeyForNewClass());
            
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
            return builder.ExtractTable(FromClass, NewClass, Properties, ForeignKeyColumns, true);
        }


        public override ModelTransformation Inverse()
        {
            return null;
        }


        private AddOneToOneForeignKeyAssociationTransformation GetAssociationTransformation()
        {
            var principal = new AssociationMemberInfo(FromClass, RelationshipMultiplicity.One, new NavigationPropertyCodeModel(NewClass));
            var dependent = new AssociationMemberInfo(NewClass, RelationshipMultiplicity.One, new NavigationPropertyCodeModel(FromClass));

            return new AddOneToOneForeignKeyAssociationTransformation(principal, dependent, ForeignKeyColumns, true);
        }

        private ScalarPropertyCodeModel GetPrimaryKeyForNewClass()
        {
            var prop = new ScalarPropertyBuilder().Int();
            prop.Name = "Id";
            return prop;
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
