using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Model
{
    public sealed class SimpleAssociationEnd
    {
        public string ClassName { get; private set; }
        public string NavigationPropertyName { get; private set; }

        public SimpleAssociationEnd(string className, string navigationPropertyName)
        {
            this.ClassName = className;
            this.NavigationPropertyName = navigationPropertyName;
        }

        public SimpleAssociationEnd(string className)
            : this(className, null)
        {
        }

    }
}
