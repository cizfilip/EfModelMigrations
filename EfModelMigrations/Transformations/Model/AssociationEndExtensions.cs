using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Model
{
    public static class AssociationEndExtensions
    {
        public static SimpleAssociationEnd ToSimpleAssociationEnd(this AssociationEnd associationEnd)
        {
            return new SimpleAssociationEnd(
                associationEnd.ClassName,
                associationEnd.HasNavigationProperty ? associationEnd.NavigationProperty.Name : null
                );
        }
    }
}
