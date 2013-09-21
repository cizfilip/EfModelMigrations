using EfModelMigrations.Exceptions;
using EfModelMigrations.Runtime.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners
{
    [Serializable]
    internal abstract class BaseRunner
    {
        public static readonly string ResultKey = "result";
        

        public string ProjectAssemblyPath { get; set; }

        

        public abstract void Run();


        protected void Return(object returnObject)
        {
            AppDomain.CurrentDomain.SetData(BaseRunner.ResultKey, returnObject);
        }


        protected Assembly LoadProjectAssembly()
        {
            return LoadAssembly(ProjectAssemblyPath);
        }

        private static Assembly LoadAssembly(string assemblyPath)
        {
            try
            {
                return Assembly.Load(AssemblyName.GetAssemblyName(assemblyPath));
            }
            catch (FileNotFoundException ex)
            {
                throw new ModelMigrationsException(
                    //TODO: Zlepšit formátování stringů z resourců
                    String.Format(Resources.BaseRunner_AssemblyNotFound, ex.FileName),
                    ex);
            }
        }
    }
}
