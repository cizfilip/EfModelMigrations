using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Extensions
{
    internal static class CodeFunction2Extensions
    {
        public static void InsertAtEnd(this CodeFunction2 method, string code)
        {
            var editPoint = method.GetEndPoint(vsCMPart.vsCMPartBody).CreateEditPoint();

            editPoint.Insert(code);
            editPoint.Insert(Environment.NewLine);

            FormatMethodCode(method);
        }

        public static void InsertAtStart(this CodeFunction2 method, string code)
        {
            var editPoint = method.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();

            editPoint.Insert(code);
            editPoint.Insert(Environment.NewLine);

            FormatMethodCode(method);
        }

        private static void FormatMethodCode(CodeFunction2 method)
        {
            method.StartPoint.CreateEditPoint().SmartFormat(method.EndPoint);
        }
    }
}
