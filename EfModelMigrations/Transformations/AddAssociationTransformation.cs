using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Resources;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public abstract class AddAssociationTransformation : ModelTransformation
    {
        public AssociationCodeModel Model { get; private set; }

        public AddAssociationTransformation(AssociationCodeModel model)
        {
            Check.NotNull(model, "model");

            this.Model = model;

            if (!Model.Principal.HasNavigationProperty && !Model.Dependent.HasNavigationProperty)
            {
                throw new ModelTransformationValidationException(Strings.Transformations_NavigationPropertyMissing);
            }
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            var modelChangeOperations = CreateModelChangeOperations(modelProvider);

            return modelChangeOperations.Concat(new[] { new AddMappingInformationOperation(CreateAssociationMapping(modelProvider)) });
        }

        protected virtual IEnumerable<IModelChangeOperation> CreateModelChangeOperations(IClassModelProvider modelProvider)
        {
            if (Model.Principal.HasNavigationProperty)
            {
                yield return new AddPropertyToClassOperation(Model.Principal.ClassName, Model.Principal.NavigationProperty);
            }

            if (Model.Dependent.HasNavigationProperty)
            {
                yield return new AddPropertyToClassOperation(Model.Dependent.ClassName, Model.Dependent.NavigationProperty);
            }
        }

        protected virtual AddAssociationMapping CreateAssociationMapping(IClassModelProvider modelProvider)
        {
            return new AddAssociationMapping(Model);
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
