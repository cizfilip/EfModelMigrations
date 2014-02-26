using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    //TODO: jak to bude s defaultnima hodnotama pro nove tridy/property?
    public class ClassCodeModel
    {
        //TODO: change to internal - az bude InternalsVisibleTo
        public ClassCodeModel(string @namespace,
            string name,
            CodeModelVisibility? visibility,
            string baseType,
            IEnumerable<string> implementedInterfaces,
            IEnumerable<PropertyCodeModel> properties)
        {
            //TODO: defaults must be supplied from configuration
            //TODO: throw if name, namespace or model is null !!!!!
            Namespace = @namespace;
            Name = name;
            Visibility = visibility ?? CodeModelVisibility.Public;
            BaseType = baseType;
            ImplementedInterfaces = implementedInterfaces ?? Enumerable.Empty<string>();
            Properties = properties ?? Enumerable.Empty<PropertyCodeModel>();
        }

        public string Namespace { get; private set; }
        public string Name { get; private set; }
        public CodeModelVisibility Visibility { get; private set; }
        public string BaseType { get; private set; }
        public IEnumerable<string> ImplementedInterfaces { get; private set; }

        public IEnumerable<PropertyCodeModel> Properties { get; private set; }

        public string FullName
        {
            get
            {
                return Namespace + "." + Name;
            }
        }
    }

    
}
