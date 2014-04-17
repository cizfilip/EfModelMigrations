using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    internal class PlaceholderOperation : SqlOperation
    {
        private static readonly string PlaceholderSql = "-- EFModelMigrationsPlaceholderOperation";

        public PlaceholderOperation(MigrationOperation underlayingOperation)
            :base(PlaceholderSql, null)
        {
            this.UnderlayingOperation = underlayingOperation;
        }

        public MigrationOperation UnderlayingOperation { get; set; }

        public override MigrationOperation Inverse
        {
            get
            {
                var inverse = UnderlayingOperation.Inverse;
                if(inverse != null)
                {
                    return new PlaceholderOperation(inverse);
                }
                return null;
            }
        }
    }
}
