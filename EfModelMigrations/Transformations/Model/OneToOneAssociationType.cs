using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Model
{
    public enum OneToOneAssociationType
    {
        DependentRequired,
        BothEndsRequired,
        BothEndsOptional
    }
}
