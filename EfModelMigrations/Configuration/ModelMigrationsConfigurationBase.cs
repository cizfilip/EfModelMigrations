using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Reflection;
using System.Resources;
using EfModelMigrations.Infrastructure.EntityFramework;

namespace EfModelMigrations.Configuration
{
    //TODO: Pridat podporu pro defaulty pro generovani kodu (napr. defaultni viditelnost trid, predek ci interface pro vsechny nove tridy apod.)
    public class ModelMigrationsConfigurationBase
    {
        public static readonly string DefaultModelMigrationsDirectory = "ModelMigrations";

        private ICodeGenerator codeGenerator;


        public ModelMigrationsConfigurationBase()
        {
            ModelMigrationGenerator = new CSharpModelMigrationGenerator();
            GeneratorDefaults = CodeGeneratorDefaults.Create();
            ModelMigrationsNamespace = GetType().Namespace;
            ModelMigrationsAssembly = GetType().Assembly;
            ModelMigrationsDirectory = ModelMigrationsConfigurationBase.DefaultModelMigrationsDirectory;
        }

        //Generators
        public IModelMigrationGenerator ModelMigrationGenerator { get; set; }

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

        public CodeGeneratorDefaults GeneratorDefaults { get; set; }

        public string ModelMigrationsNamespace { get; set; }
        //TODO: Kontrola zdali se nezadava rooted path
        public string ModelMigrationsDirectory { get; set; }
        public Assembly ModelMigrationsAssembly { get; set; }

        public string ModelNamespace { get; set; }

        public Type DbMigrationsConfigurationType { get; set; }

        private DbMigrationsConfiguration dbMigrationsConfiguration;
        public DbMigrationsConfiguration DbMigrationsConfiguration
        {
            get
            {
                if(dbMigrationsConfiguration == null)
                {
                    if(DbMigrationsConfigurationType == null)
                    {
                        throw new InvalidOperationException("Cannot create db migrations configuration, DbMigrationsConfigurationType was not specified."); //TODO: string do resourcu
                    }

                    dbMigrationsConfiguration = DbMigrationsConfigurationType.CreateInstance<DbMigrationsConfiguration>();

                    dbMigrationsConfiguration.CodeGenerator = new ExtendedCSharpMigrationCodeGenerator();
                    dbMigrationsConfiguration.SetSqlGenerator("System.Data.SqlClient", new ExtendedSqlServerMigrationSqlGenerator());
                }
                return dbMigrationsConfiguration;
            }
        }
        
        //TODO: Prepsat az bude hotova komopnenta pro update resource souboru s jiz aplikovanymi migracemi
        public IEnumerable<string> GetAppliedMigrations()
        {
            ResourceManager resources = new ResourceManager(GetType());
            return resources.GetString("AppliedMigrations").Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
