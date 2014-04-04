using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
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
            IEnumerable<ScalarPropertyCodeModel> properties,
            IEnumerable<NavigationPropertyCodeModel> navigationProperties,
            IEnumerable<ScalarPropertyCodeModel> primaryKeys)
        {
            Check.NotEmpty(name, "name");

            //TODO: defaults must be supplied from configuration
            Name = name;
            Visibility = visibility;
            BaseType = baseType;
            ImplementedInterfaces = implementedInterfaces ?? Enumerable.Empty<string>();
            Properties = properties ?? Enumerable.Empty<ScalarPropertyCodeModel>();
            NavigationProperties = navigationProperties ?? Enumerable.Empty<NavigationPropertyCodeModel>();
            PrimaryKeys = primaryKeys ?? Enumerable.Empty<ScalarPropertyCodeModel>();
        }

        public string Name { get; private set; }
        public CodeModelVisibility Visibility { get; private set; }
        public string BaseType { get; private set; }
        public IEnumerable<string> ImplementedInterfaces { get; private set; }

        public IEnumerable<ScalarPropertyCodeModel> Properties { get; private set; }

        public IEnumerable<NavigationPropertyCodeModel> NavigationProperties { get; private set; }

        public IEnumerable<ScalarPropertyCodeModel> PrimaryKeys { get; private set; }

        public EntityType StoreEntityType { get; internal set; }

        public EntityType ConceptualEntityType { get; internal set; }
    }

    
}
