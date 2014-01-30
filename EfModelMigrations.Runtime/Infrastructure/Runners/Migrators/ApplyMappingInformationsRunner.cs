using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.Migrators
{
    [Serializable]
    internal class ApplyMappingInformationsRunner : MigratorBaseRunner
    {
        public bool IsRevert { get; set; }

        public override void Run()
        {
            var classModelProvider = GetClassModelProvider();

            IEnumerable<IMappingInformation> informationsToAdd = GetModelTransformations(IsRevert).SelectMany(t => t.GetMappingInformationsToAdd(classModelProvider));
            IEnumerable<IMappingInformation> informationsToRemove = GetModelTransformations(IsRevert).SelectMany(t => t.GetMappingInformationsToRemove(classModelProvider));

            string dbContextFullName = DbConfiguration.ContextType.FullName;

            IMappingInformationsProvider mappingProvider;
            
            //var modelChangesExecutor = new VsModelChangesExecutor(ModelProject,
            //    Configuration.ModelNamespace,
            //    dbContextFullName,
            //    Configuration.CodeGenerator
            //    );

            try
            {
                //modelChangesExecutor.Execute(operations);
            }
            catch (Exception e) //TODO: mozna catch jenom ModelMigrationsException
            {
                //TODO: dodelat revert provedenech zmen v pripade ze dojde k chybe
                throw new ModelMigrationsException("Error during aplying mapping informations! See inner exception. (Note: temporary model is in broken state now)", e);
            }
            
        }

        
    }
}
