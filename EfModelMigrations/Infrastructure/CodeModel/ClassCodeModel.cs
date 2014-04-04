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
            IEnumerable<PrimitivePropertyCodeModel> properties,
            IEnumerable<NavigationPropertyCodeModel> navigationProperties,
            IEnumerable<PrimitivePropertyCodeModel> primaryKeys)
        {
            Check.NotEmpty(name, "name");

            //TODO: defaults must be supplied from configuration
            Name = name;
            Visibility = visibility;
            BaseType = baseType;
            ImplementedInterfaces = implementedInterfaces ?? Enumerable.Empty<string>();
            Properties = properties ?? Enumerable.Empty<PrimitivePropertyCodeModel>();
            NavigationProperties = navigationProperties ?? Enumerable.Empty<NavigationPropertyCodeModel>();
            PrimaryKeys = primaryKeys ?? Enumerable.Empty<PrimitivePropertyCodeModel>();
        }

        public string Name { get; private set; }
        public CodeModelVisibility Visibility { get; private set; }
        public string BaseType { get; private set; }
        public IEnumerable<string> ImplementedInterfaces { get; private set; }

        public IEnumerable<PrimitivePropertyCodeModel> Properties { get; private set; }

        public IEnumerable<NavigationPropertyCodeModel> NavigationProperties { get; private set; }

        public IEnumerable<PrimitivePropertyCodeModel> PrimaryKeys { get; private set; }

        public EntityType StoreEntityType { get; internal set; }

        public EntityType ConceptualEntityType { get; internal set; }
    }

    
}
