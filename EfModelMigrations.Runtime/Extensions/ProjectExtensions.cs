using EfModelMigrations.Exceptions;
using EfModelMigrations.Runtime.Properties;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Extensions
{
    public static class ProjectExtensions
    {
        public static bool TryBuild(this Project project)
        {
            var dte = project.DTE;
            var configuration = dte.Solution.SolutionBuild.ActiveConfiguration.Name;

            dte.Solution.SolutionBuild.BuildProject(configuration, project.UniqueName, true);

            return dte.Solution.SolutionBuild.LastBuildInfo == 0;
        }

        public static string GetTargetDir(this Project project)
        {
            var fullPath = project.GetProjectDir();
            
            //TODO: Pro web project je outputPath jiný ("Bin") - Netřeba
            //Vyzkoušeno v experimentech i pro web project vrací OutputPath správnou hodnotu
            var outputPath = project.GetConfigurationPropertyValue<string>("OutputPath");

            return Path.Combine(fullPath, outputPath);
        }


        public static string GetProjectDir(this Project project)
        {
            return project.GetPropertyValue<string>("FullPath");
        }

        public static string GetAssemblyPath(this Project project)
        {
            return Path.Combine(GetTargetDir(project), project.GetPropertyValue<string>("OutputFileName"));
        }

        public static string GetAssemblyName(this Project project)
        {
            return project.GetPropertyValue<string>("AssemblyName");
        }

        public static string GetRootNamespace(this Project project)
        {
            return project.GetPropertyValue<string>("RootNamespace");
        }

        public static void AddContentToProject(this Project project, string relativePath, string content)
        {
            if (Path.IsPathRooted(relativePath))
                throw new ArgumentException(Resources.ProjectExtensions_PathMustBeRelative, "relativePath");

            string absolutePath = Path.Combine(project.GetProjectDir(), relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
            File.WriteAllText(absolutePath, content);
            project.ProjectItems.AddFromFile(absolutePath);
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
