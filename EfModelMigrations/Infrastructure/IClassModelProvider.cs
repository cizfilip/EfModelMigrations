using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure
{
    //TODO: pokusit se uplne vyhodit z projektu (proste to nikde nepouzvat)
    public interface IClassModelProvider
    {
        ClassCodeModel GetClassCodeModel(string className);

        
    }
}
