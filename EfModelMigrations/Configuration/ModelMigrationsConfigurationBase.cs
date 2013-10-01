using EfModelMigrations.Infrastructure.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Configuration
{
    public class ModelMigrationsConfigurationBase
    {
        public static readonly string DefaultModelMigrationsDirectory = "ModelMigrations";

        public ModelMigrationsConfigurationBase()
        {
            ModelMigrationGenerator = new CSharpModelMigrationGenerator();
            CodeGenerator = new CSharpCodeGenerator();
            ModelMigrationsNamespace = GetType().Namespace;
            ModelMigrationsAssembly = GetType().Assembly;
            ModelMigrationsDirectory = ModelMigrationsConfigurationBase.DefaultModelMigrationsDirectory;
        }

        public IModelMigrationGenerator ModelMigrationGenerator { get; set; }
        public ICodeGenerator CodeGenerator { get; set; }

        public string ModelMigrationsNamespace { get; set; }
        //TODO: Kontrola zdali se nezadava rooted path
        public string ModelMigrationsDirectory { get; set; }
        public Assembly ModelMigrationsAssembly { get; set; }

        public string ModelNamespace { get; set; }

        //TODO: Prepsat az bude hotova komopnenta pro update resource souboru s jiz aplikovanymi migracemi
        public IEnumerable<string> GetAppliedMigrations()
        {
            return Enumerable.Empty<string>();
            //Resources = new ResourceManager(GetType());
            //Resources.GetString("AppliedMigrations");
        }

    }
}
