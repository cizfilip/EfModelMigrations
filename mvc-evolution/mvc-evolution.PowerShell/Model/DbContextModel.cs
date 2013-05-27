using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc_evolution.PowerShell.Model
{
    class DbContextModel
    {
        public string Name { get; set; }
        public string Namespace { get; set; }

        public IEnumerable<DbSetPropertyModel> DbSetProperties { get; set; }
    }
}
