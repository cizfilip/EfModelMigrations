using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Configuration
{
    public class ModelMigrationsConfiguration<TEfMigrationsConfiguration> : ModelMigrationsConfigurationBase
        where TEfMigrationsConfiguration : DbMigrationsConfiguration
    {
        public ModelMigrationsConfiguration()
        {
            DbMigrationsConfigurationType = typeof(TEfMigrationsConfiguration);
        }
    }
}
