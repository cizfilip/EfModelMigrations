using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel.Builders
{
    public class NavigationPropertyBuilder : IFluentInterface
    {
        public NavigationProperty ToOne(string targetClassName)
        {
            return NavigationProperty.Default(targetClassName);
        }

        public NavigationProperty ToMany(string targetClassName)
        {
            return NavigationProperty.DefaultCollection(targetClassName);
        }
    }
}
