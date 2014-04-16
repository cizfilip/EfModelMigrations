using System.Data.Entity;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Entity.Infrastructure.DependencyResolution;

namespace EfModelMigrations.Infrastructure.CodeModel.Builders
{
    public abstract class NavigationPropertyBuilder
    {
        private string targetClass;

        public NavigationPropertyBuilder(string targetClass)
        {
            this.targetClass = targetClass;
        }

        protected NavigationPropertyCodeModel Build(bool isCollection,
            string name = null, 
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            NavigationPropertyCodeModel property;
            if (name == null)
            {
                property = new NavigationPropertyCodeModel(targetClass, isCollection);
            }
            else
            {
                property = new NavigationPropertyCodeModel(name, targetClass, isCollection);
            }
            property.IsVirtual = isVirtual;
            property.IsSetterPrivate = isSetterPrivate;
            property.Visibility = visibility;

            return property;
        }
    }

    public interface IOneNavigationPropertyBuilder : IFluentInterface
    {
        NavigationPropertyCodeModel One(string name, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        NavigationPropertyCodeModel One(CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
    }

    public sealed class OneNavigationPropertyBuilder : NavigationPropertyBuilder, IOneNavigationPropertyBuilder
    {
        public OneNavigationPropertyBuilder(string targetClass)
            :base(targetClass)
        {
        }

        public NavigationPropertyCodeModel One(string name, 
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return Build(false, name, visibility, isVirtual, isSetterPrivate);
        }

        public NavigationPropertyCodeModel One(CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return Build(false, null, visibility, isVirtual, isSetterPrivate);
        }
    }

    public interface IManyNavigationPropertyBuilder : IFluentInterface
    {
        NavigationPropertyCodeModel Many(string name, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        NavigationPropertyCodeModel Many(CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
    }

    public sealed class ManyNavigationPropertyBuilder : NavigationPropertyBuilder, IManyNavigationPropertyBuilder
    {
        public ManyNavigationPropertyBuilder(string targetClass)
            :base(targetClass)
        {
        }

        public NavigationPropertyCodeModel Many(string name,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return Build(true, name, visibility, isVirtual, isSetterPrivate);
        }

        public NavigationPropertyCodeModel Many(CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return Build(true, null, visibility, isVirtual, isSetterPrivate);
        }
    }
}
