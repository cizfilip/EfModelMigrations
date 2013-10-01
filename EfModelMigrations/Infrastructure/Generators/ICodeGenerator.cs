using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    public interface ICodeGenerator
    {
        string GenerateEmptyClass(ClassCodeModel classModel);
        string GenerateProperty(PropertyCodeModel propertyModel);

        string GetFileExtensions();
    }
}
