using EfModelMigrations.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    public abstract class CodeGenerator : ICodeGenerator
    {
        protected CodeGeneratorDefaults defaults;
        protected IMappingInformationsGenerator mappingGenerator;

        public CodeGenerator(CodeGeneratorDefaults defaults, IMappingInformationsGenerator mappingGenerator)
        {
            this.defaults = defaults;
            this.mappingGenerator = mappingGenerator;
        }

        public abstract string GenerateEmptyClass(string name, string @namespace, CodeModel.CodeModelVisibility? visibility, string baseType, IEnumerable<string> implementedInterfaces);

        public abstract string GenerateProperty(CodeModel.PropertyCodeModel propertyModel, out string propertyName);

        public abstract string GenerateDbSetProperty(string className, out string dbSetPropertyName);

        public abstract string GetFileExtensions();

        public virtual IMappingInformationsGenerator MappingGenerator
        {
            get
            {
                return mappingGenerator;
            }
        }
    }
}
