using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EfModelMigrations.Extensions;
using EfModelMigrations.Runtime.Resources;

namespace EfModelMigrations.Runtime.Extensions
{
    internal static class ProjectExtensions
    {
        public const int S_OK = 0;
        public const string WebApplicationProjectTypeGuid = "{349C5851-65DF-11DA-9384-00065B846F21}";
        public const string WebSiteProjectTypeGuid = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";

        public static bool TryBuild(this Project project)
        {
            var dte = project.DTE;
            var configuration = dte.Solution.SolutionBuild.ActiveConfiguration.Name;

            dte.Solution.SolutionBuild.BuildProject(configuration, project.UniqueName, true);

            return dte.Solution.SolutionBuild.LastBuildInfo == 0;
        }

        public static void Build(this Project project, Func<Exception> exceptionFactory)
        {
            if (!TryBuild(project))
            {
                throw exceptionFactory();
            }
        }

        public static string GetTargetDir(this Project project)
        {
            var fullPath = project.GetProjectDir();

            var outputPath
                = project.IsWebSiteProject()
                      ? "Bin"
                      : project.GetConfigurationPropertyValue<string>("OutputPath");

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

        public static ProjectItem AddContentToProject(this Project project, string relativePath, string content)
        {
            if (Path.IsPathRooted(relativePath))
                throw new ArgumentException(Strings.ProjectExtensions_PathMustBeRelative, "relativePath");

            string absolutePath = Path.Combine(project.GetProjectDir(), relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
            File.WriteAllText(absolutePath, content);
            return project.ProjectItems.AddFromFile(absolutePath);
        }

        public static ProjectItem AddContentToProjectFromAbsolutePath(this Project project, string absolutePath, string content)
        {
            if (!Path.IsPathRooted(absolutePath))
                throw new ArgumentException(Strings.ProjectExtensions_PathMustBeAbsolute, "absolutePath");

            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
            File.WriteAllText(absolutePath, content);
            return project.ProjectItems.AddFromFile(absolutePath);

            //TODO: po pridani mozna pouzivat metodu .SmartFormat na editPointu, která snad dela to co ctrl+k,d
        }

        public static ProjectItem AddFileToProject(this Project project, string path)
        {
            return project.ProjectItems.AddFromFile(path);
        }

        public static string GetFileName(this Project project, string projectItemName)
        {
            ProjectItem projectItem;

            try
            {
                projectItem = project.ProjectItems.Item(projectItemName);
            }
            catch
            {
                return Path.Combine(project.GetProjectDir(), projectItemName);
            }

            return projectItem.FileNames[0];
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

        public static bool IsWebProject(this Project project)
        {
            return project.GetProjectTypes().Any(
                g => g.EqualsOrdinalIgnoreCase(WebApplicationProjectTypeGuid)
                     || g.EqualsOrdinalIgnoreCase(WebSiteProjectTypeGuid));
        }

        public static bool IsWebSiteProject(this Project project)
        {
            return project.GetProjectTypes().Any(g => g.EqualsOrdinalIgnoreCase(WebSiteProjectTypeGuid));
        }

        // <summary>
        // Gets all aggregate project type GUIDs for the given project.
        // Note that when running in Visual Studio app domain (which is how this code is used in
        // production) a shellVersion of 10 is fine because VS has binding redirects to cause the
        // latest version to be loaded. When running tests is may be desirable to explicitly pass
        // a different version. See CodePlex 467.
        // </summary>
        public static IEnumerable<string> GetProjectTypes(this Project project, int shellVersion = 10)
        {
            IVsHierarchy hierarchy;

            var serviceProviderType = Type.GetType(string.Format(
                "Microsoft.VisualStudio.Shell.ServiceProvider, Microsoft.VisualStudio.Shell, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                shellVersion));

            var serviceProvider = (IServiceProvider)Activator.CreateInstance(
                serviceProviderType,
                (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)project.DTE);

            var solution = (IVsSolution)serviceProvider.GetService(typeof(IVsSolution));
            var hr = solution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);

            if (hr != S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            string projectTypeGuidsString;

            var aggregatableProject = (IVsAggregatableProject)hierarchy;
            hr = aggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuidsString);

            if (hr != S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            return projectTypeGuidsString.Split(';');
        }
    }
}
