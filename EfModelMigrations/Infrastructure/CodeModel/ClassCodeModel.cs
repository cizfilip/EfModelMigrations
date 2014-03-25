using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public class ClassCodeModel
    {
        internal ClassCodeModel(
            string name,
            CodeModelVisibility visibility,
            string baseType,
            IEnumerable<string> implementedInterfaces,
            IEnumerable<ScalarProperty> properties,
            IEnumerable<NavigationProperty> navigationProperties,
            IEnumerable<ScalarProperty> primaryKeys)
        {
            Check.NotEmpty(name, "name");

            //TODO: defaults must be supplied from configuration
            Name = name;
            Visibility = visibility;
            BaseType = baseType;
            ImplementedInterfaces = implementedInterfaces ?? Enumerable.Empty<string>();
            Properties = properties ?? Enumerable.Empty<ScalarProperty>();
            NavigationProperties = navigationProperties ?? Enumerable.Empty<NavigationProperty>();
            PrimaryKeys = primaryKeys ?? Enumerable.Empty<ScalarProperty>();
        }

        public string Name { get; private set; }
        public CodeModelVisibility Visibility { get; private set; }
        public string BaseType { get; private set; }
        public IEnumerable<string> ImplementedInterfaces { get; private set; }

        public IEnumerable<ScalarProperty> Properties { get; private set; }

        public IEnumerable<NavigationProperty> NavigationProperties { get; private set; }

        public IEnumerable<ScalarProperty> PrimaryKeys { get; private set; }

        
    }

    
}
