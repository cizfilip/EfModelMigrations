using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Runtime.Extensions;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Runtime.Properties;

namespace EfModelMigrations.Runtime.Infrastructure
{
    internal class VsProjectBuilder
    {
        private Project modelProject;

        public VsProjectBuilder(Project modelProject)
        {
            this.modelProject = modelProject;
        }

        public void BuildModelProject()
        {
            modelProject.Build(() => new ModelMigrationsException(Resources.CannotBuildProject));
        }
    }
}
