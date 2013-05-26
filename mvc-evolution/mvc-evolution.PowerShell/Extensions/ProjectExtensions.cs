using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace mvc_evolution.PowerShell.Extensions
{
    internal static class ProjectExtensions
    {

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

        private static T GetPropertyValue<T>(this Project project, string propertyName)
        {

            var property = project.Properties.Item(propertyName);

            if (property == null)
            {
                return default(T);
            }

            return (T)property.Value;
        }

    }
}
