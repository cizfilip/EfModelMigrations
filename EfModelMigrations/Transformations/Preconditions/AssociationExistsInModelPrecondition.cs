using EfModelMigrations.Infrastructure;
using EfModelMigrations.Extensions;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Resources;

namespace EfModelMigrations.Transformations.Preconditions
{
    public class AssociationExistsInModelPrecondition : ModelTransformationPrecondition
    {
        private SimpleAssociationEnd principal;
        private SimpleAssociationEnd dependent;

        public AssociationExistsInModelPrecondition(SimpleAssociationEnd principal, SimpleAssociationEnd dependent)
        {
            Check.NotNull(principal, "principal");
            Check.NotNull(dependent, "dependent");

            this.principal = principal;
            this.dependent = dependent;
        }

        public override VerificationResult Verify(IClassModelProvider modelProvider)
        {
            try
            {
                if (principal.HasNavigationPropertyName)
                {
                    if(!NavigationPropertyExistInModel(principal, modelProvider))
                    {
                        return VerificationResult.Error(Strings.Precondition_AssociationNotExists(principal.ClassName, dependent.ClassName));
                    }
                }

                if (dependent.HasNavigationPropertyName)
                {
                    if (!NavigationPropertyExistInModel(dependent, modelProvider))
                    {
                        return VerificationResult.Error(Strings.Precondition_AssociationNotExists(principal.ClassName, dependent.ClassName));
                    }
                }                
            }
            catch (Exception) { }

            return VerificationResult.Successful();
        }

        private bool NavigationPropertyExistInModel(SimpleAssociationEnd associationEnd, IClassModelProvider modelProvider)
        {
            return modelProvider.GetClassCodeModel(associationEnd.ClassName)
                        .NavigationProperties
                        .Any(p => string.Equals(p.Name, associationEnd.NavigationPropertyName, StringComparison.Ordinal));
        }
    }
}
