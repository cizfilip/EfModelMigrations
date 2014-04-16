using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Extensions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Runtime.Infrastructure.Migrations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Resources;
using EfModelMigrations.Utilities;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
        protected DbMigrationsConfiguration DbConfiguration
        {
            get
            {
                return Configuration.DbMigrationsConfiguration;
            }
        }

        private DbContext dbContext;
        protected DbContext DbContext
        {
            get
            {
                if (dbContext == null)
                {
                    dbContext = DbConfiguration.ContextType.CreateInstance<DbContext>();
                }
                return dbContext;
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
                    Strings.BaseRunner_AssemblyNotFound(ex.FileName),
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
                    Strings.BaseRunner_AssemblyNotFound(ex.FileName),
                    ex);
            }
        }

        private ModelMigrationsConfigurationBase CreateConfiguration()
        {
            Type configType;
            var typeFinder = new TypeFinder();
            if(typeFinder.TryFindModelMigrationsConfigurationType(ProjectAssembly, out configType))
            {
                return configType.CreateInstance<ModelMigrationsConfigurationBase>();
            }
            else
            {
                throw new ModelMigrationsException(Strings.CannotFindConfiguration);
            }
        }

        //protected T CreateInstance<T>(Type type, object[] constructorParameters = null)
        //{
        //    try
        //    {
        //        return (T)Activator.CreateInstance(type, constructorParameters);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ModelMigrationsException(Strings.CannotCreateInstance(type.Name), e);
        //    }
        //}

        
       
    }
}
