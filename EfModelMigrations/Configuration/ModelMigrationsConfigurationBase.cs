using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Reflection;
using System.Resources;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Resources;
using System.IO;

namespace EfModelMigrations.Configuration
{
    //TODO: Pridat podporu pro defaulty pro generovani kodu (napr. defaultni viditelnost trid, predek ci interface pro vsechny nove tridy apod.)
    public class ModelMigrationsConfigurationBase
    {
        public static readonly string DefaultModelMigrationsDirectory = "ModelMigrations";

        public CodeGeneratorDefaults generatorDefaults;
        public CodeGeneratorDefaults GeneratorDefaults
        {
            get { return generatorDefaults; }
            set
            {
                Check.NotNull(value, "GeneratorDefaults");
                generatorDefaults = value;
            }
        }

        public string modelMigrationsNamespace;
        public string ModelMigrationsNamespace
        {
            get { return modelMigrationsNamespace; }
            set
            {
                Check.NotEmpty(value, "ModelMigrationsNamespace");
                modelMigrationsNamespace = value;
            }
        }

        public string modelMigrationsDirectory;
        public string ModelMigrationsDirectory
        {
            get { return modelMigrationsDirectory; }
            set
            {
                Check.NotEmpty(value, "ModelMigrationsDirectory");

                if (Path.IsPathRooted(value))
                {
                    throw new ArgumentException(Strings.ModelMigrationsDirectoryIsRooted(value));
                }

                modelMigrationsDirectory = value;
            }
        }

        public Assembly modelMigrationsAssembly;
        public Assembly ModelMigrationsAssembly
        {
            get { return modelMigrationsAssembly; }
            set
            {
                Check.NotNull(value, "ModelMigrationsAssembly");
                modelMigrationsAssembly = value;
            }
        }

        public string modelNamespace;
        public string ModelNamespace
        {
            get { return modelNamespace; }
            set
            {
                Check.NotEmpty(value, "ModelNamespace");
                modelNamespace = value;
            }
        }

        private Type dbMigrationsConfigurationType;
        public Type DbMigrationsConfigurationType
        {
            get { return dbMigrationsConfigurationType; }
            set
            {
                Check.NotNull(value, "DbMigrationsConfigurationType");

                if (!typeof(DbMigrationsConfiguration).IsAssignableFrom(value))
                {
                    throw new ArgumentException(Strings.DbMigrationsConfigurationTypeIsInvalid(value.Name));
                }

                dbMigrationsConfigurationType = value;
            }
        }

        private DbMigrationsConfiguration dbMigrationsConfiguration;
        public DbMigrationsConfiguration DbMigrationsConfiguration
        {
            get
            {
                if (dbMigrationsConfiguration == null)
                {
                    if (DbMigrationsConfigurationType == null)
                    {
                        throw new InvalidOperationException(Strings.DbMigrationsConfigurationTypeNotSpecified);
                    }

                    dbMigrationsConfiguration = DbMigrationsConfigurationType.CreateInstance<DbMigrationsConfiguration>();

                    dbMigrationsConfiguration.CodeGenerator = new ExtendedCSharpMigrationCodeGenerator();
                    //TODO: sql generator by se mel nastavovat rovnou v ef db configuraci tak aby se dalo v pohode volat Update-Database i mimo nas framework
                    dbMigrationsConfiguration.SetSqlGenerator("System.Data.SqlClient", new ExtendedSqlServerMigrationSqlGenerator());
                }
                return dbMigrationsConfiguration;
            }
        }

        //Generators
        private IModelMigrationGenerator modelMigrationGenerator;
        public IModelMigrationGenerator ModelMigrationGenerator { 
            get { return modelMigrationGenerator; }
            set
            {
                Check.NotNull(value, "ModelMigrationGenerator");
                modelMigrationGenerator = value;
            }
        }

        private ICodeGenerator codeGenerator;
        public ICodeGenerator CodeGenerator
        {
            get
            {
                if (codeGenerator == null)
                {
                    codeGenerator = new CSharpCodeGenerator(GeneratorDefaults, new CSharpMappingInformationsGenerator());
                }
                return codeGenerator;
            }
            set
            {
                Check.NotNull(value, "CodeGenerator");
                codeGenerator = value;
            }
        }

        public ModelMigrationsConfigurationBase()
        {
            ModelMigrationGenerator = new CSharpModelMigrationGenerator();
            GeneratorDefaults = CodeGeneratorDefaults.Create();
            ModelMigrationsNamespace = GetType().Namespace;
            ModelMigrationsAssembly = GetType().Assembly;
            ModelMigrationsDirectory = ModelMigrationsConfigurationBase.DefaultModelMigrationsDirectory;
        }

        //TODO: Prepsat az bude hotova komopnenta pro update resource souboru s jiz aplikovanymi migracemi
        public IEnumerable<string> GetAppliedMigrations()
        {
            ResourceManager resources = new ResourceManager(GetType());
            return resources.GetString("AppliedMigrations").Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
