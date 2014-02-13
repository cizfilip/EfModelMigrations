using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class OneToOneAssociationInfo : IMappingInformation
    {
        public AssociationMemberInfo Principal { get; private set; }
        public AssociationMemberInfo Dependent { get; private set; }
        
        public OneToOneAssociationInfo(AssociationMemberInfo principal, AssociationMemberInfo dependent)
        {
            this.Principal = principal;
            this.Dependent = dependent;
        }


    }
}
