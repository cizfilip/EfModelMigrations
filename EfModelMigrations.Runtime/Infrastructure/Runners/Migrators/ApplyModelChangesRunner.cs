using EfModelMigrations.Exceptions;
using EfModelMigrations.Operations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.Migrators
{
    [Serializable]
    internal class ApplyModelChangesRunner : MigratorBaseRunner
    {
        public HistoryTracker HistoryTracker { get; set; }
        public bool IsRevert { get; set; }
        public bool Force { get; set; }

        public override void Run()
        {
            var transformations = GetModelTransformations(IsRevert);

            if (transformations.Where(t => t.IsDestructiveChange).Any() && !Force)
            {
                throw new ModelMigrationsException(string.Format("Some operations in migration {0} may cause data loss in database! If you really want to execute this migration rerun the migrate command with -Force parameter.")); //TODO: string do resourcu
            }
            
            var classModelProvider = GetClassModelProvider();

            IEnumerable<IModelChangeOperation> operations = transformations.SelectMany(t => t.GetModelChangeOperations(classModelProvider));
            
            string dbContextFullName = DbConfiguration.ContextType.FullName;         

            var modelChangesExecutor = new VsModelChangesExecutor(HistoryTracker,
                ModelProject, 
                Configuration.ModelNamespace, 
                dbContextFullName,
                Configuration.CodeGenerator
                );

            modelChangesExecutor.Execute(operations);    
        }
    }
}
