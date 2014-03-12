using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Extensions
{
    internal static class EnvDteExtensions
    {
        public static int AllvsEPReplaceTextOptionsFlags()
        {
            return (int)(vsEPReplaceTextOptions.vsEPReplaceTextAutoformat | vsEPReplaceTextOptions.vsEPReplaceTextNormalizeNewlines | vsEPReplaceTextOptions.vsEPReplaceTextTabsSpaces | vsEPReplaceTextOptions.vsEPReplaceTextKeepMarkers);
        }
    }
}
