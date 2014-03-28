using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public abstract class AddAssociationTransformation : ModelTransformation
    {
        public AssociationEnd Principal { get; private set; }
        public AssociationEnd Dependent { get; private set; }

        public AddAssociationTransformation(AssociationEnd principal, AssociationEnd dependent)
        {
            this.Principal = principal;
            this.Dependent = dependent;

            //TODO: stringy do resourců
            if (principal.NavigationProperty == null && dependent.NavigationProperty == null)
            {
                throw new ModelTransformationValidationException("You must specify at least one navigation property in association.");
            }
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            var modelChangeOperations = CreateModelChangeOperations();

            return modelChangeOperations.Concat(new[] { new AddMappingInformationOperation(CreateMappingInformation()) });
        }

        protected virtual IEnumerable<IModelChangeOperation> CreateModelChangeOperations()
        {
            if (Principal.NavigationProperty != null)
            {
                yield return new AddPropertyToClassOperation(Principal.ClassName, Principal.NavigationProperty);
            }

            if (Dependent.NavigationProperty != null)
            {
                yield return new AddPropertyToClassOperation(Dependent.ClassName, Dependent.NavigationProperty);
            }
        }

        protected abstract AddAssociationMapping CreateMappingInformation();
       
        public override ModelTransformation Inverse()
        {
            return new RemoveAssociationTransformation(Principal.ToSimpleAssociationEnd(), Dependent.ToSimpleAssociationEnd());
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
