using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Runtime.Infrastructure.Migrations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Runtime.Properties;
using EfModelMigrations.Utilities;
using EnvDTE;
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

        public RunnerLogger Log { get; set; }
        public string ProjectAssemblyPath { get; set; }

        private Assembly projectAssembly;
        protected Assembly ProjectAssembly
        {
            get
            {
                if (projectAssembly == null)
                {
                    projectAssembly = LoadAssemblyFromPath(ProjectAssemblyPath);
                }
                return projectAssembly;
            }
        }

        private ModelMigrationsConfigurationBase configuration;
        protected ModelMigrationsConfigurationBase Configuration
        {
            get
            {
                if (configuration == null)
                {
                    configuration = CreateConfiguration();
                }
                return configuration;
            }
        }
                
        
        public abstract void Run();


        protected void Return(object returnObject)
        {
            AppDomain.CurrentDomain.SetData(BaseRunner.ResultKey, returnObject);
        }


        private Assembly LoadAssembly(string name)
        {
            try
            {
                return Assembly.Load(name);
            }
            catch (FileNotFoundException ex)
            {
                throw new ModelMigrationsException(
                    //TODO: Zlepšit formátování stringů z resourců
                    String.Format(Resources.BaseRunner_AssemblyNotFound, ex.FileName),
                    ex);
            }
        }


        private Assembly LoadAssemblyFromPath(string assemblyPath)
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

        private ModelMigrationsConfigurationBase CreateConfiguration()
        {
            Type configType;
            var typeFinder = new TypeFinder();
            if(typeFinder.TryFindModelMigrationsConfigurationType(ProjectAssembly, out configType))
            {
                return CreateInstance<ModelMigrationsConfigurationBase>(configType);
            }
            else
            {
                throw new ModelMigrationsException(Resources.CannotFindConfiguration);
            }
        }

        protected T CreateInstance<T>(Type type, object[] constructorParameters = null)
        {
            try
            {
                return (T)Activator.CreateInstance(type, constructorParameters);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.CannotCreateInstance, type.Name), e);
            }
        }

        
       
    }
}
