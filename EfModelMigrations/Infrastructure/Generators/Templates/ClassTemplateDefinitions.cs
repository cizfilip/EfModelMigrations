using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators.Templates
{
    internal partial class ClassTemplate
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public CodeModelVisibility Visibility { get; set; }
        public string BaseType { get; set; }

        private IEnumerable<string> implementedInterfaces;
        public IEnumerable<string> ImplementedInterfaces
        {
            get
            {
                return implementedInterfaces;
            }
            set
            {
                implementedInterfaces = value ?? Enumerable.Empty<string>();
            }
        }

        public IEnumerable<string> Imports { get; set; }
        public Func<CodeModelVisibility, string> CodeModelVisibilityMapper { get; set; }

        private string GetBasesListString()
        {
            if (string.IsNullOrEmpty(BaseType) && !ImplementedInterfaces.Any())
            {
                return "";
            }

            var basesString = string.Join(", ", GetBasesList());

            return string.Format(" : {0}", basesString);
        }

        private IEnumerable<string> GetBasesList()
        {
            if (!string.IsNullOrEmpty(BaseType))
            {
                yield return BaseType;
            }

            foreach (var @interface in ImplementedInterfaces)
            {
                yield return @interface;
            }
        }
    }
}
