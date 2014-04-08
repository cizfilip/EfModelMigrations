using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Model
{
    public static class MultiplicityHelper
    {
        public static bool IsManyToMany(AssociationEnd source, AssociationEnd target)
        {
            return source.Multipticity == RelationshipMultiplicity.Many && target.Multipticity == RelationshipMultiplicity.Many;
        }

        public static bool IsOneToMany(AssociationEnd source, AssociationEnd target)
        {
            return (source.Multipticity == RelationshipMultiplicity.One || source.Multipticity == RelationshipMultiplicity.ZeroOrOne) 
                && target.Multipticity == RelationshipMultiplicity.Many;
        }

        public static bool IsOneToOne(AssociationEnd source, AssociationEnd target)
        {
            return (source.Multipticity == RelationshipMultiplicity.One || source.Multipticity == RelationshipMultiplicity.ZeroOrOne)
                &&
                (target.Multipticity == RelationshipMultiplicity.One || target.Multipticity == RelationshipMultiplicity.ZeroOrOne);
        }
    }
}
