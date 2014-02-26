using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Model
{
    public sealed class AssociationMemberInfo
    {
        public AssociationMemberInfo(string className, NavigationPropertyCodeModel navigationProperty)
        {
            this.ClassName = className;
            this.NavigationProperty = navigationProperty;
        }

        public string ClassName { get; private set; }
        public NavigationPropertyCodeModel NavigationProperty { get; private set; }

        public string NavigationPropertyName
        {
            get
            {
                return NavigationProperty != null ? NavigationProperty.Name : null;
            }
        }
    }
}
