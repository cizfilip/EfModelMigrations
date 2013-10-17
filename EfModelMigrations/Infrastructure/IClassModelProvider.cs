using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure
{
    public interface IClassModelProvider
    {
        ClassCodeModel GetClassCodeModel(string className);

        ClassCodeModel CreateClassCodeModel(string name, 
            CodeModelVisibility? visibility, 
            string baseType, 
            IEnumerable<string> implementedInterfaces,
            IEnumerable<PropertyCodeModel> properties);

        
    }
}
