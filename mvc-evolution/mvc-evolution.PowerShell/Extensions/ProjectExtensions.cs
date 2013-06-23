using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace mvc_evolution.PowerShell.Extensions
{
    internal static class ProjectExtensions
    {
        public const string WebSiteProjectTypeGuid = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";

        public static string GetProjectDir(this Project project)
        {
            return project.GetPropertyValue<string>("FullPath");
        }

        public static string GetRootNamespace(this Project project)
        {
            return project.GetPropertyValue<string>("RootNamespace");
            //or default namespace ??
            //return project.GetPropertyValue<string>("DefaultNamespace");
        }

        public static void AddContentToProject(this Project project, string path, string content)
        {
            File.WriteAllText(path, content);
            project.ProjectItems.AddFromFile(path);
        }

        public static void AddFileToProject(this Project project, string path)
        {
            project.ProjectItems.AddFromFile(path);
        }

        private static T GetPropertyValue<T>(this Project project, string propertyName)
        {

            var property = project.Properties.Item(propertyName);

            if (property == null)
            {
                return default(T);
            }

            return (T)property.Value;
        }

        public static bool TryBuild(this Project project)
        {
            var dte = project.DTE;
            var configuration = dte.Solution.SolutionBuild.ActiveConfiguration.Name;

            dte.Solution.SolutionBuild.BuildProject(configuration, project.UniqueName, true);

            return dte.Solution.SolutionBuild.LastBuildInfo == 0;
        }

        public static string GetAssemblyName(this Project project)
        {
            return project.GetPropertyValue<string>("AssemblyName");
        }

        public static string GetAssemblyPath(this Project project)
        {
            return Path.Combine(project.GetTargetDir(), project.GetPropertyValue<string>("OutputFileName"));
        }

        public static string GetTargetDir(this Project project)
        {
            var fullPath = project.GetProjectDir();


            //TODO: WebProject Check
            //var outputPath
            //    = project.IsWebSiteProject()
            //          ? "Bin"
            //          : project.GetConfigurationPropertyValue<string>("OutputPath");

            var outputPath = project.GetConfigurationPropertyValue<string>("OutputPath");

            return Path.Combine(fullPath, outputPath);
        }

        private static T GetConfigurationPropertyValue<T>(this Project project, string propertyName)
        {
           
            var property = project.ConfigurationManager.ActiveConfiguration.Properties.Item(propertyName);

            if (property == null)
            {
                return default(T);
            }

            return (T)property.Value;
        }

    
        
    }
}
