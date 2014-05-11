using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Resources;
using EfModelMigrations.Transformations.Model;
using EfModelMigrations.Transformations.Preconditions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public abstract class RemoveAssociationTransformation : TransformationWithInverse
    {
        public SimpleAssociationEnd Principal { get; private set; }
        public SimpleAssociationEnd Dependent { get; private set; }

        public RemoveAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, AddAssociationTransformation inverse) : base(inverse)
        {
            Check.NotNull(principal, "source");
            Check.NotNull(dependent, "target");

            this.Principal = principal;
            this.Dependent = dependent;

            if (!principal.HasNavigationPropertyName && !dependent.HasNavigationPropertyName)
            {
                throw new ModelTransformationValidationException(Strings.Transformations_NavigationPropertyMissing);
            }
        }

        public RemoveAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent)
            : this(principal, dependent, null)
        {
        }

        public override IEnumerable<ModelTransformationPrecondition> GetPreconditions()
        {
            yield return new ClassExistsInModelPrecondition(Principal.ClassName);
            yield return new ClassExistsInModelPrecondition(Dependent.ClassName);
            yield return new AssociationExistsInModelPrecondition(Principal, Dependent);
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            if (!string.IsNullOrEmpty(Principal.NavigationPropertyName))
            {
                yield return new RemovePropertyFromClassOperation(Principal.ClassName, Principal.NavigationPropertyName);
            }

            if (!string.IsNullOrEmpty(Dependent.NavigationPropertyName))
            {
                yield return new RemovePropertyFromClassOperation(Dependent.ClassName, Dependent.NavigationPropertyName);
            }

            yield return new RemoveMappingInformationOperation(new RemoveAssociationMapping(Principal, Dependent));
        }

        public override bool IsDestructiveChange
        {
            get { return true; }
        }
    }
}
