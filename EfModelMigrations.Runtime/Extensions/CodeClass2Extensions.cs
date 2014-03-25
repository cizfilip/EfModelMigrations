using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Extensions
{
    public static class CodeClass2Extensions
    {
        public static CodeProperty2 FindProperty(this CodeClass2 codeClass, string propertyName)
        {
            try
            {
                return (CodeProperty2)codeClass.Members.Item(propertyName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static CodeFunction2 FindMethod(this CodeClass2 codeClass, string methodName)
        {
            try
            {
                return (CodeFunction2)codeClass.Members.Item(methodName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
