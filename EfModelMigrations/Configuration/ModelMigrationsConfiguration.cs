using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Configuration
{
    public class ModelMigrationsConfiguration<TEfMigrationsConfiguration> : ModelMigrationsConfigurationBase
    {
        public ModelMigrationsConfiguration()
        {
            DbMigrationsConfigurationType = typeof(TEfMigrationsConfiguration);
        }
    }
}
