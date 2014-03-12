using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Configuration
{
    public class CodeGeneratorDefaults
    {
        //TODO: dodelat dalsi defaultni hodnoty

        public ClassDefaults Class { get; set; }
        public PropertyDefaults Property { get; set; }

        public static CodeGeneratorDefaults Create()
        {
            return new CodeGeneratorDefaults()
            {
                Class = new ClassDefaults()
                {
                    Visibility = CodeModelVisibility.Public
                },
                Property = new PropertyDefaults()
                {
                    Visibility = CodeModelVisibility.Public,
                    IsSetterPrivate = false,
                    IsVirtual = false
                }
            };
        }
        
    }

    public class ClassDefaults
    {
        public CodeModelVisibility Visibility { get; set; }
    }

    public class PropertyDefaults
    {
        public CodeModelVisibility Visibility { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsSetterPrivate { get; set; }

    }
}
