using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public class ClassCodeModel
    {
        internal ClassCodeModel(string @namespace,
            string name,
            CodeModelVisibility? visibility,
            string baseType,
            IEnumerable<string> implementedInterfaces,
            IEnumerable<ScalarProperty> properties)
        {
            //TODO: defaults must be supplied from configuration
            //TODO: throw if name, namespace is null !!!!!
            Namespace = @namespace;
            Name = name;
            Visibility = visibility;
            BaseType = baseType;
            ImplementedInterfaces = implementedInterfaces ?? Enumerable.Empty<string>();
            Properties = properties ?? Enumerable.Empty<ScalarProperty>();
        }

        public string Namespace { get; private set; }
        public string Name { get; private set; }
        public CodeModelVisibility? Visibility { get; private set; }
        public string BaseType { get; private set; }
        public IEnumerable<string> ImplementedInterfaces { get; private set; }

        public IEnumerable<ScalarProperty> Properties { get; private set; }

        
    }

    
}
