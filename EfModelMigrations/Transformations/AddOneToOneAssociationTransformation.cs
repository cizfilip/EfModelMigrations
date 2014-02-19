using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;

namespace EfModelMigrations.Transformations
{
    public class AddOneToOneAssociationTransformation : ModelTransformation
    {
        public AssociationMemberInfo Principal { get; private set; }
        public AssociationMemberInfo Dependent { get; private set; }

        public bool BothEndsRequired { get; private set; }

        public bool WillCascadeOnDelete { get; private set; }

        public AddOneToOneAssociationTransformation(AssociationMemberInfo principal, AssociationMemberInfo dependent, bool bothEndsRequired, bool willCascadeOnDelete)
        {
            this.Principal = principal;
            this.Dependent = dependent;
            this.WillCascadeOnDelete = willCascadeOnDelete;
            this.BothEndsRequired = bothEndsRequired;

            //TODO: stringy do resourců
            if (principal.NavigationProperty == null && dependent.NavigationProperty == null)
            {
                throw new ModelTransformationValidationException("You must specify at least one navigation property in association.");
            }
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            if (Principal.NavigationProperty != null)
            {
                yield return new AddPropertyToClassOperation(Principal.ClassName, Principal.NavigationProperty.ToPropertyCodeModel());
            }

            if (Dependent.NavigationProperty != null)
            {
                yield return new AddPropertyToClassOperation(Dependent.ClassName, Dependent.NavigationProperty.ToPropertyCodeModel());
            }

            yield return new AddMappingInformationOperation(CreateMappingInformation());
        }

        private AssociationMappingInformation CreateMappingInformation()
        {
            if (Principal.NavigationProperty != null) //Navigation property on Principal
            {
                if (BothEndsRequired)
                {
                    return new HasRequiredWithRequiredPrincipalInfo(Principal.ClassName, Principal.NavigationPropertyName, Dependent.ClassName, Dependent.NavigationPropertyName, WillCascadeOnDelete);
                }
                else
                {
                    return new HasOptionalWithRequiredInfo(Principal.ClassName, Principal.NavigationPropertyName, Dependent.ClassName, Dependent.NavigationPropertyName, WillCascadeOnDelete);
                }
            }
            else //Navigation property on Dependent
            {
                if(BothEndsRequired)
                {
                    return new HasRequiredWithRequiredDependentInfo(Dependent.ClassName, Dependent.NavigationPropertyName, Principal.ClassName, Principal.NavigationPropertyName, WillCascadeOnDelete);
                }
                else
                {
                    return new HasRequiredWithOptionalInfo(Dependent.ClassName, Dependent.NavigationPropertyName, Principal.ClassName, Principal.NavigationPropertyName, WillCascadeOnDelete);
                }
            }
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            return builder.OneToOneRelationOperations(Principal.ClassName, Dependent.ClassName, WillCascadeOnDelete);
        }

        public override ModelTransformation Inverse()
        {
            return null;
        }
    }

    public sealed class AssociationMemberInfo
    {
        public AssociationMemberInfo(string className, NavigationPropertyCodeModel navigationProperty)
        {
            this.ClassName = className;
            this.NavigationProperty = navigationProperty;
        }

        public string ClassName { get; private set; }
        public NavigationPropertyCodeModel NavigationProperty { get; private set; }

        public string NavigationPropertyName
        {
            get
            {
                return NavigationProperty != null ? NavigationProperty.Name : null;
            }
        }
    }


}
