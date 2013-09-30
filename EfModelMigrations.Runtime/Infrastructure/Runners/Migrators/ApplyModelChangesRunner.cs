using EfModelMigrations.Operations;
using EfModelMigrations.Transformations;
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

            try
            {
                foreach (var operation in operations)
                {
                    //TODO: insert god object as param
                    operation.ExecuteModelChanges();

                    executedOperations.Add(operation);
                }
            }
            catch (Exception) //TODO: mozna catch jenom ModelMigrationsException
            {
                foreach (var operation in executedOperations.Select(o => o.Inverse()).Reverse())
                {
                    //TODO: insert god object as param
                    operation.ExecuteModelChanges();
                }

                throw;
            }


        }
    }
}
