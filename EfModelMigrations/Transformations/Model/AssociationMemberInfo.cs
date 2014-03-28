using EfModelMigrations.Infrastructure.CodeModel;
using System.Data.Entity.Core.Metadata.Edm;

namespace EfModelMigrations.Transformations.Model
{
    public sealed class AssociationMemberInfo
    {
        public AssociationMemberInfo(string className, RelationshipMultiplicity multipticity, NavigationPropertyCodeModel navigationProperty)
        {
            this.ClassName = className;
            this.NavigationProperty = navigationProperty;
            this.Multipticity = multipticity;
        }

        public AssociationMemberInfo(string className, RelationshipMultiplicity multipticity)
            :this(className, multipticity, null)
        {
        }

        public string ClassName { get; private set; }
        public NavigationPropertyCodeModel NavigationProperty { get; private set; }

        public RelationshipMultiplicity Multipticity { get; private set; }

    }
}
