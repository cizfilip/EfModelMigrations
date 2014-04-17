using EfModelMigrations.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    public abstract class MigrationOperationWithInverse : MigrationOperation
    {
        protected MigrationOperation inverse;

        public MigrationOperationWithInverse(MigrationOperation inverse, object anonymousArguments = null)
            :base(anonymousArguments)
        {
            this.inverse = inverse;
        }

        public override MigrationOperation Inverse
        {
            get
            {
                if (inverse == null)
                {
                    throw new InvalidOperationException(Strings.MigrationOperationInverseMissing);
                }
                return inverse;
            }
        }

        
    }
}
