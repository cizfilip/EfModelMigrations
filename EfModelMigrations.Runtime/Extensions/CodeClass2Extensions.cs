using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Extensions
{
    internal static class CodeClass2Extensions
    {
        public static CodeProperty2 FindProperty(this CodeClass2 codeClass, string propertyName)
        {
            return codeClass.FindMember<CodeProperty2>(propertyName);
        }

        public static CodeFunction2 FindMethod(this CodeClass2 codeClass, string methodName)
        {
            return codeClass.FindMember<CodeFunction2>(methodName);
        }

        public static T FindMember<T>(this CodeClass2 codeClass, string memberName) where T : class
        {
            try
            {
                return codeClass.Members.Item(memberName) as T;
            }
            catch (Exception)
            {
                return (T)null;
            }
        }
    }
}
