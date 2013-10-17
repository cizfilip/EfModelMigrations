using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public class RenameClassOperation : ModelChangeOperation
    {
        private ClassCodeModel classModel;
        private string newName;

        public RenameClassOperation(ClassCodeModel classModel, string newName)
        {
            this.classModel = classModel;
            this.newName = newName;
        }

        public override void ExecuteModelChanges(IModelChangesProvider provider)
        {
            provider.RenameClass(classModel, newName);
        }

        //TODO: ne moc hezky jeste jednou poresit pouzivani CodeClassModelu napric projektem....
        public override ModelChangeOperation Inverse()
        {
            return new RenameClassOperation(
                new ClassCodeModel(
                    classModel.Namespace,
                    newName,
                    classModel.Visibility,
                    classModel.BaseType,
                    classModel.ImplementedInterfaces,
                    classModel.Properties
                    ),
                classModel.Name);
        }
    }
}
