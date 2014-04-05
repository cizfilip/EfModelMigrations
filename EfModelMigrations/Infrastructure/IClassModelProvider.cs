using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure
{
    //TODO: sem mozna pridat nejake metody ci objekt co usnadnuje validace tj. ClassExists PropertyExists apod... - pouziti hlavne v remove transformacich
    public interface IClassModelProvider
    {
        ClassCodeModel GetClassCodeModel(string className);

        bool IsEnumInModel(string enumName);

        EfModel EfModel { get; }
    }
}
