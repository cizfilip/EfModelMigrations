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

        public override void Run()
        {
            string oldEdmxModel = GetEdmxModelAsString();

            var classModelProvider = GetClassModelProvider();

            IEnumerable<IModelChangeOperation> operations = GetModelTransformations(IsRevert).SelectMany(t => t.GetModelChangeOperations(classModelProvider));
            
            string dbContextFullName = DbConfiguration.ContextType.FullName;         

            var modelChangesExecutor = new VsModelChangesExecutor(HistoryTracker,
                ModelProject, 
                Configuration.ModelNamespace, 
                dbContextFullName,
                Configuration.CodeGenerator
                );

            modelChangesExecutor.Execute(operations);    
            
            Return(oldEdmxModel);
        }

        private string GetEdmxModelAsString()
        {
            using (var writer = new StringWriter())
            {
                GetEdmxModel().Save(writer);
                return writer.ToString();
            }
        }
    }
}
