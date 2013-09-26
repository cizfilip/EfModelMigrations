using EfModelMigrations.Infrastructure.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
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
            ModelMigrationsNamespace = GetType().Namespace;
            ModelMigrationsDirectory = ModelMigrationsConfigurationBase.DefaultModelMigrationsDirectory;
        }

        public IModelMigrationGenerator ModelMigrationGenerator { get; set; }

        public string ModelMigrationsNamespace { get; set; }
        public string ModelMigrationsDirectory { get; set; }
    }
}
