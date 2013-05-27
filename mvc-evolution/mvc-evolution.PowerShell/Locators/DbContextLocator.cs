using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace mvc_evolution.PowerShell.Locators
{
    internal static class DbContextLocator
    {

        private static readonly string[] ExcludedNamespaces = new[] { "Microsoft", "MS", "System" };
        /// <summary>
        /// Finds ewery class in project which derives from System.Data.Entity.DbContext
        /// </summary>
        public static IEnumerable<CodeType> FindDbContextsInProject(Project project, DomainCommand cmd)
        {
            var codeElements = project.CodeModel.CodeElements;
            return FindDbContextsFromCodeElements(codeElements, cmd);           
        }

        private static IEnumerable<CodeType> FindDbContextsFromCodeElements(CodeElements codeElements, DomainCommand cmd)
        {
            List<CodeType> results = new List<CodeType>();

            foreach (CodeElement codeElement in codeElements)
            {
                //cmd.WriteLine(codeElement.Name + "   " + (codeElement is CodeType).ToString() + "   " + codeElement.InfoLocation.ToString());

                if (codeElement is CodeType &&
                    codeElement.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject && 
                    ((CodeType)codeElement).IsDerivedFrom["System.Data.Entity.DbContext"])
                {
                    //cmd.WriteLine("AAAAA" + codeElement.Name);
                    results.Add(codeElement as CodeType);
                }

                CodeElements childrenToSearch = null;
                if ((codeElement is CodeClass) && (codeElement.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject))
                {
                    childrenToSearch = ((CodeClass)codeElement).Members;
                }
                else if (codeElement is CodeNamespace)
                {
                    var codeNamespace = (CodeNamespace)codeElement;
                    if (!ExcludedNamespaces.Contains(codeNamespace.FullName, StringComparer.Ordinal))
                        childrenToSearch = codeNamespace.Members;
                }
                if (childrenToSearch != null)
                {
                    var childResults = FindDbContextsFromCodeElements(childrenToSearch, cmd);
                    //cmd.WriteLine(childrenToSearch.Count.ToString());
                    results.AddRange(childResults);
                }
            }

            return results;
        }
    }
}
