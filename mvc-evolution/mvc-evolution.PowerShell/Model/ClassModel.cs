using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc_evolution.PowerShell.Model
{
    internal class ClassModel
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public IEnumerable<PropertyModel> Properties { get; set; }

    }
}
