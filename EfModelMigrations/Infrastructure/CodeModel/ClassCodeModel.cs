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
        public ClassCodeModel()
        {
            //TODO: defaults mst be supplied from configuration
            Namespace = null;
            Name = null;
            Visibility = CodeModelVisibility.Public;
            BaseType = null;
            ImplementedInterfaces = Enumerable.Empty<string>();
            Properties = Enumerable.Empty<PropertyCodeModel>();
        }

        //public IEnumerable<string> Imports { get; set; }

        //TODO: Namespace?? vlastne ho nepouzivam protoze se vzdy prepisuje namespacem modeloveho projektu
        public string Namespace { get; set; }
        public string Name { get; set; }
        public CodeModelVisibility Visibility { get; set; }
        public string BaseType { get; set; }
        public IEnumerable<string> ImplementedInterfaces { get; set; }

        public IEnumerable<PropertyCodeModel> Properties { get; set; }

        
    }

    
}
