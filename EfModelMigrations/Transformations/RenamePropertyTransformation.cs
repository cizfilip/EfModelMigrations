using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Transformations.Preconditions;

namespace EfModelMigrations.Transformations
{
    //TODO: Je rozdil kdyz prejmenovavam scalarPropert, navigation property nebo primary key!!!
    //TODO: a co mapovací informace pro přejmenovávanou property ???
    public class RenamePropertyTransformation : ModelTransformation
    {
        public string ClassName { get; private set; }
        public string OldName { get; private set; }
        public string NewName { get; private set; }

        public RenamePropertyTransformation(string className, string oldName, string newName)
        {
            Check.NotEmpty(className, "className");
            Check.NotEmpty(oldName, "oldName");
            Check.NotEmpty(newName, "newName");

            this.ClassName = className;
            this.OldName = oldName;
            this.NewName = newName;
        }

        public override IEnumerable<ModelTransformationPrecondition> GetPreconditions()
        {
            yield return new ClassExistsInModelPrecondition(ClassName);
            yield return new PropertiesExistInClassPrecondition(ClassName, new[] { OldName });
            yield return new PropertiesNotExistInClassPrecondition(ClassName, new[] { NewName });
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new RenamePropertyOperation(ClassName, OldName, NewName);
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            yield return builder.RenameColumnOperation(
                builder.NewModel.GetStoreEntitySetForClass(ClassName),                
                builder.OldModel.GetStoreColumnForProperty(ClassName, OldName),
                builder.NewModel.GetStoreColumnForProperty(ClassName, NewName)
                );
        }

        public override ModelTransformation Inverse()
        {
            return new RenamePropertyTransformation(ClassName, NewName, OldName);
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
