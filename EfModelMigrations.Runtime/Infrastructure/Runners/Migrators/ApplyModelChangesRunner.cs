using EfModelMigrations.Operations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Transformations;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.Migrators
{
    [Serializable]
    internal class ApplyModelChangesRunner : MigratorBaseRunner
    {
        public bool IsRevert { get; set; }

        public override void Run()
        {
            IEnumerable<ModelChangeOperation> operations = GetModelTransformations(IsRevert).SelectMany(t => t.GetModelChangeOperations());

            List<ModelChangeOperation> executedOperations = new List<ModelChangeOperation>();

            //TODO: obejit se bez dynamic
            string dbContextFullName = CreateInstance<dynamic>(Configuration.EfMigrationsConfigurationType).ContextType.FullName;         


            var modelChangesProvider = new VsModelChangesProvider(ModelProject, 
                Configuration.ModelNamespace, 
                dbContextFullName,
                Configuration.CodeGenerator
                );

            try
            {
                foreach (var operation in operations)
                {
                    operation.ExecuteModelChanges(modelChangesProvider);

                    executedOperations.Add(operation);
                }
            }
            catch (Exception) //TODO: mozna catch jenom ModelMigrationsException
            {
                foreach (var operation in executedOperations.Select(o => o.Inverse()).Reverse())
                {
                    operation.ExecuteModelChanges(modelChangesProvider);
                }

                throw;
            }
        }
    }
}
