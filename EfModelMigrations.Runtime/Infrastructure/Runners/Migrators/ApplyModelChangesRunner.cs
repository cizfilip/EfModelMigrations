using EfModelMigrations.Operations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Transformations;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.Migrators
{
    //TODO: aplikovat i mapping change operace!
    [Serializable]
    internal class ApplyModelChangesRunner : MigratorBaseRunner
    {
        public bool IsRevert { get; set; }

        public override void Run()
        {
            string oldEdmxModel = GetEdmxModelAsString();

            var classModelProvider = GetClassModelProvider();

            IEnumerable<ModelChangeOperation> operations = GetModelTransformations(IsRevert).SelectMany(t => t.GetModelChangeOperations(classModelProvider));

            List<ModelChangeOperation> executedOperations = new List<ModelChangeOperation>();

            string dbContextFullName = DbConfiguration.ContextType.FullName;         

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
            finally
            {
                Return(oldEdmxModel);
            }
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
