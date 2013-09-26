using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Templates
{
    internal partial class DbContextTemplate
    {
        public string Namespace { get; set; }
        public string ContextName { get; set; }
    }
}
