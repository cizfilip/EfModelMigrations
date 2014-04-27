using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public class AssociationCodeModel
    {
        public AssociationEnd Principal { get; private set; }
        public AssociationEnd Dependent { get; private set; }

        private IDictionary<string, AssociationInfo> informations;

        public AssociationCodeModel(AssociationEnd principal, AssociationEnd dependent)
        {
            this.Principal = principal;
            this.Dependent = dependent;
            this.informations = new Dictionary<string, AssociationInfo>();
        }

        public void AddInformation(AssociationInfo info)
        {
            this.informations[info.Name] = info;
        }

        public AssociationInfo<TValue> GetInformation<TValue>(string name)
        {
            AssociationInfo info;
            if (informations.TryGetValue(name, out info))
            {
                return info as AssociationInfo<TValue>;
            }
            return null;
        }
        
        public bool IsManyToMany()
        {
            return Principal.Multipticity == RelationshipMultiplicity.Many && Dependent.Multipticity == RelationshipMultiplicity.Many;
        }

        public bool IsOneToMany()
        {
            return (Principal.Multipticity == RelationshipMultiplicity.One || Principal.Multipticity == RelationshipMultiplicity.ZeroOrOne)
                && Dependent.Multipticity == RelationshipMultiplicity.Many;
        }

        public bool IsOneToOne()
        {
            return (Principal.Multipticity == RelationshipMultiplicity.One || Principal.Multipticity == RelationshipMultiplicity.ZeroOrOne)
                &&
                (Dependent.Multipticity == RelationshipMultiplicity.One || Dependent.Multipticity == RelationshipMultiplicity.ZeroOrOne);
        }
    }
}
