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
        public ClassCodeModel ClassModel { get; set; }
        public IEnumerable<string> Imports { get; set; }
        public Func<CodeModelVisibility, string> CodeModelVisibilityMapper { get; set; }

        private string GetBasesListString()
        {
            if (string.IsNullOrEmpty(ClassModel.BaseType) && !ClassModel.ImplementedInterfaces.Any())
            {
                return "";
            }

            var basesString = string.Join(", ", GetBasesList());

            return string.Format(" : {0}", basesString);
        }

        private IEnumerable<string> GetBasesList()
        {
            if (!string.IsNullOrEmpty(ClassModel.BaseType))
            {
                yield return ClassModel.BaseType;
            }

            foreach (var @interface in ClassModel.ImplementedInterfaces)
            {
                yield return @interface;
            }
        }
    }
}
