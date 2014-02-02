using EfModelMigrations.Infrastructure.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Configuration
{
    //TODO: Pridat podporu pro defaulty pro generovani kodu (napr. defaultni viditelnost trid, predek ci interface pro vsechny nove tridy apod.)
    public class ModelMigrationsConfigurationBase
    {
        public static readonly string DefaultModelMigrationsDirectory = "ModelMigrations";

        public ModelMigrationsConfigurationBase()
        {
            ModelMigrationGenerator = new CSharpModelMigrationGenerator();
            CodeGenerator = new CSharpCodeGenerator(new CSharpMappingInformationsGenerator());
            ModelMigrationsNamespace = GetType().Namespace;
            ModelMigrationsAssembly = GetType().Assembly;
            ModelMigrationsDirectory = ModelMigrationsConfigurationBase.DefaultModelMigrationsDirectory;
        }

        //Generators
        public IModelMigrationGenerator ModelMigrationGenerator { get; set; }
        public ICodeGenerator CodeGenerator { get; set; }

        public string ModelMigrationsNamespace { get; set; }
        //TODO: Kontrola zdali se nezadava rooted path
        public string ModelMigrationsDirectory { get; set; }
        public Assembly ModelMigrationsAssembly { get; set; }

        public string ModelNamespace { get; set; }

        public Type EfMigrationsConfigurationType { get; set; }

        //TODO: Prepsat az bude hotova komopnenta pro update resource souboru s jiz aplikovanymi migracemi
        public IEnumerable<string> GetAppliedMigrations()
        {
            ResourceManager resources = new ResourceManager(GetType());
            return resources.GetString("AppliedMigrations").Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
