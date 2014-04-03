using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    public abstract class IdentityOperation : MigrationOperation
    {
        private string principalTable;
        private ColumnModel principalColumn;

        private readonly List<DependentColumn> dependentColumns = new List<DependentColumn>();

        public IdentityOperation(object anonymousArguments = null)
            : base(anonymousArguments)
        { }

        public string PrincipalTable
        {
            get { return principalTable; }
            set
            {
                principalTable = value;
            }
        }
        public ColumnModel PrincipalColumn
        {
            get { return principalColumn; }
            set
            {
                principalColumn = value;
            }
        }
        public List<DependentColumn> DependentColumns
        {
            get
            {
                return dependentColumns;
            }
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }

    public class DependentColumn
    {
        public string DependentTable { get; set; }
        public string ForeignKeyColumn { get; set; }
    }
}
