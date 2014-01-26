using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure
{
    public interface IDbContextChangesProvider
    {
        void AddDbSetPropertyForClass(string classNameForAddProperty);
        void RemoveDbSetPropertyForClass(string classNameForRemoveProperty);
    }
}
