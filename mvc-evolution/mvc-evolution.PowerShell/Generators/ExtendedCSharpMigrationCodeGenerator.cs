using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc_evolution.PowerShell.Generators
{
    class ExtendedCSharpMigrationCodeGenerator : CSharpMigrationCodeGenerator
    {
        private IEnumerable<MigrationOperation> operations;

        public ExtendedCSharpMigrationCodeGenerator(IEnumerable<MigrationOperation> operations)
        {
            this.operations = operations;
        }

        protected override string Generate(IEnumerable<MigrationOperation> operations, string @namespace, string className)
        {
            return base.Generate(this.operations, @namespace, className);
        }


    }
}
