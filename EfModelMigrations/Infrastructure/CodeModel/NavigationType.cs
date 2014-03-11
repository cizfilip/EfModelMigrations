using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public class NavigationType : CodeModelType
    {
        public string TargetClass { get; private set; }

        public bool IsCollection { get; private set; }


        public NavigationType(string targetClass, bool isCollection)
        {
            this.TargetClass = targetClass;
            this.IsCollection = isCollection;
        }
    }
}
