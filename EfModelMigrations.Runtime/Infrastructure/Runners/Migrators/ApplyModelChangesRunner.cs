using EfModelMigrations.Exceptions;
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

            IEnumerable<IModelChangeOperation> operations = GetModelTransformations(IsRevert).SelectMany(t => t.GetModelChangeOperations(classModelProvider));
            
            string dbContextFullName = DbConfiguration.ContextType.FullName;         

            var modelChangesExecutor = new VsModelChangesExecutor(ModelProject, 
                Configuration.ModelNamespace, 
                dbContextFullName,
                Configuration.CodeGenerator
                );

            try
            {
                modelChangesExecutor.Execute(operations);    
            }
            catch (Exception e) //TODO: mozna catch jenom ModelMigrationsException
            {
                //TODO: dodelat revert provedenech zmen v pripade ze dojde k chybe
                throw new ModelMigrationsException("Error during aplying model changes! See inner exception. (Note: temporary model is in broken state now)", e);
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
