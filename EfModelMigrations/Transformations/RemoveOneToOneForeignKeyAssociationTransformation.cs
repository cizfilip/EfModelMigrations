using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class RemoveOneToOneForeignKeyAssociationTransformation : RemoveAssociationWithForeignKeyTransformation
    {
        public RemoveOneToOneForeignKeyAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, AddOneToOneForeignKeyAssociationTransformation inverse)
            : base(principal, dependent, inverse)
        {
        }

        public RemoveOneToOneForeignKeyAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent)
            : this(principal, dependent, null)
        {
        }
    }
}
