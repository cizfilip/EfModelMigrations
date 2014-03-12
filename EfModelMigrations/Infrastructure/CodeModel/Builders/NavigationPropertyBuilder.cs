using System.Data.Entity;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Entity.Infrastructure.DependencyResolution;

namespace EfModelMigrations.Infrastructure.CodeModel.Builders
{
    public class NavigationPropertyBuilder : IFluentInterface
    {
        public NavigationProperty One(string targetClassName)
        {
            return new NavigationProperty(targetClassName, targetClassName, false);
        }

        public NavigationProperty One(string name, string targetClassName)
        {
            return new NavigationProperty(name, targetClassName, false);
        }

        public NavigationProperty Many(string targetClassName)
        {
            var propertyName = DbConfiguration.DependencyResolver.GetService<IPluralizationService>().Pluralize(targetClassName);
            return new NavigationProperty(propertyName, targetClassName, true);
        }

        public NavigationProperty Many(string name, string targetClassName)
        {
            return new NavigationProperty(name, targetClassName, true);
        }
        
    }
}
