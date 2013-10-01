using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Transformations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EnvDTE;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.Migrators
{
    [Serializable]
    internal abstract class MigratorBaseRunner : BaseRunner
    {
        public string ModelMigrationId { get; set; }
        public Project ModelProject { get; set; }

        private ModelMigration modelMigration;
        protected ModelMigration ModelMigration
        {
            get
            {
                if (modelMigration == null)
                {
                    var locator = new ModelMigrationsLocator(Configuration);
                    var modelMigrationType = locator.FindModelMigration(ModelMigrationId);
                    modelMigration = CreateInstance<ModelMigration>(modelMigrationType);
                    modelMigration.ClassModelProvider = new VsClassModelProvider(ModelProject, Configuration.ModelNamespace);
                }
                return modelMigration;
            }
        }

        protected IEnumerable<ModelTransformation> GetModelTransformations(bool isRevert)
        {
            ModelMigration.Reset();
            if (isRevert)
            {
                ModelMigration.Down();
            }
            else
            {
                ModelMigration.Up();
            }

            return ModelMigration.Transformations;
        }

        public override abstract void Run();
        


    }
}
