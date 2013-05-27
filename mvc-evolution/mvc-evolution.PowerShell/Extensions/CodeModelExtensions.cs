using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace mvc_evolution.PowerShell.Extensions
{
    internal static class CodeModelExtensions
    {
        //From MvcScaffold

        /// <summary>
        /// Adds a member to a type using the supplied source code, which should be valid code in the target project's language.
        /// </summary>
        /// <param name="codeType">The class or interface to which the member will be added</param>
        /// <param name="sourceCode">The source code of the member to be added</param>
        /// <param name="position">The position in the class where the source code should be added. See documentation for EnvDTE.CodeModel.AddVariable for allowed values. 
        /// If the position is not specified, the new member will be added to the end of the class.</param>
        /// <param name="replaceTextOptions">Text formatting options. See documentation for EnvDTE.CodeModel.AddVariable for allowed values.</param>
        public static void AddMemberFromSourceCode(this CodeType codeType, string sourceCode, object position = null, vsEPReplaceTextOptions replaceTextOptions = vsEPReplaceTextOptions.vsEPReplaceTextAutoformat)
        {
            if (codeType == null) throw new ArgumentNullException("codeClass");

            CodeElement temporaryElement;
            if (codeType is CodeClass)
            {
                temporaryElement = (CodeElement)((CodeClass)codeType).AddVariable("temporaryVariable", "System.Object", Position: position ?? -1 /* Add to end of class by default */);
            }
            else if (codeType is CodeInterface)
            {
                temporaryElement = (CodeElement)((CodeInterface)codeType).AddFunction("temporaryFunction", vsCMFunction.vsCMFunctionFunction, "System.Object", Position: position ?? -1 /* Add to end of class by default */);
            }
            else
            {
                throw new ArgumentException("Parameter value must be an instance of EnvDTE.CodeClass or EnvDTE.CodeInterface", "codeType");
            }

            temporaryElement.ReplaceWithSourceCode(sourceCode, replaceTextOptions);
        }

        /// <summary>
        /// Replaces a code element with the supplied source code, which should be valid code in the target project's language.
        /// </summary>
        public static void ReplaceWithSourceCode(this CodeElement codeElement, string sourceCode, vsEPReplaceTextOptions replaceTextOptions = vsEPReplaceTextOptions.vsEPReplaceTextAutoformat)
        {
            if (codeElement == null) throw new ArgumentNullException("codeElement");
            var startPoint = codeElement.GetStartPoint();
            startPoint.CreateEditPoint().ReplaceText(codeElement.GetEndPoint(), IndentAllButFirstLine(sourceCode ?? string.Empty, startPoint.LineCharOffset - 1), (int)replaceTextOptions);
        }

        private static string IndentAllButFirstLine(string value, int charsToIndent)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            if (charsToIndent == 0)
                return value;

            var lineSeparator = Environment.NewLine + new string(' ', charsToIndent);
            var lines = value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return string.Join(lineSeparator, lines);
        }

    }
}
