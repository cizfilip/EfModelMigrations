using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class AddAssociationMapping : IAddMappingInformation
    {
        //Principal
        public AssociationMemberInfo Source { get; private set; }
        //Dependent
        public AssociationMemberInfo Target { get; private set; }

        public bool? WillCascadeOnDelete { get; set; }

        public ManyToManyJoinTable JoinTable { get; set; }

        public string[] ForeignKeyColumnNames { get; set; }

        public string[] ForeignKeyProperties { get; set; }

        public AddAssociationMapping(AssociationMemberInfo source, AssociationMemberInfo target)
        {
            this.Source = source;
            this.Target = target;
        }


        public EfFluentApiCallChain BuildEfFluentApiCallChain()
        {
            EfFluentApiCallChain callChain;

            if (Source.NavigationProperty != null) //Navigation property on Source
            {
                var methods = GetFluentApiMethodsStartingFromSource();
                callChain = new EfFluentApiCallChain(Source.ClassName)
                    .AddMethodCall(methods.Item1, CreatePropertySelectorParameter(Source.ClassName, Source.NavigationProperty))
                    .AddMethodCall(methods.Item2, CreatePropertySelectorParameter(Target.ClassName, Target.NavigationProperty));
            }
            else //Navigation property on Target
            {
                var methods = GetFluentApiMethodsStartingFromTarget();
                callChain = new EfFluentApiCallChain(Target.ClassName)
                    .AddMethodCall(methods.Item1, CreatePropertySelectorParameter(Target.ClassName, Target.NavigationProperty))
                    .AddMethodCall(methods.Item2, CreatePropertySelectorParameter(Source.ClassName, Source.NavigationProperty));
            }

            AddAdditionalMethodCalls(callChain);            

            return callChain;
        }


        protected virtual Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromSource()
        {
            var hasMethod = MapMultiplicityToHasAssociationFluentApiMethod(Target.Multipticity);
            var withMethod = MapMultiplicityToWithAssociationFluentApiMethod(Source.Multipticity);

            return FixAssociationFluentApiMethods(hasMethod, withMethod, isPrincipal: true);
        }
        protected virtual Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromTarget()
        {
            var hasMethod = MapMultiplicityToHasAssociationFluentApiMethod(Source.Multipticity);
            var withMethod = MapMultiplicityToWithAssociationFluentApiMethod(Target.Multipticity);

            return FixAssociationFluentApiMethods(hasMethod, withMethod, isPrincipal: false);
        }

        
        protected virtual void AddAdditionalMethodCalls(EfFluentApiCallChain callChain)
        {
            if (ForeignKeyProperties != null)
            {
                AddHasForeignKeyMethodCall(callChain);
            }

            if(ForeignKeyColumnNames != null)
            {
                AddMapForeignKeysMethodCall(callChain);
            }

            if(JoinTable != null)
            {
                AddMapJoinTableMethodCall(callChain);
            }

            if(WillCascadeOnDelete.HasValue)
            {
                AddCascadeOnDeleteMethodCall(callChain);
            }
        }

        #region Private Methods
        private void AddHasForeignKeyMethodCall(EfFluentApiCallChain callChain)
        {
            callChain.AddMethodCall(EfFluentApiMethods.HasForeignKey, new PropertySelectorParameter(Target.ClassName, ForeignKeyProperties));
        }

        private void AddMapForeignKeysMethodCall(EfFluentApiCallChain callChain)
        {
            callChain.AddMethodCall(EfFluentApiMethods.Map, new MapMethodParameter().MapKey(ForeignKeyColumnNames));
        }

        private void AddCascadeOnDeleteMethodCall(EfFluentApiCallChain callChain)
        {
            callChain.AddMethodCall(EfFluentApiMethods.WillCascadeOnDelete, new ValueParameter(WillCascadeOnDelete.Value));
        }

        private void AddMapJoinTableMethodCall(EfFluentApiCallChain callChain)
        {
            string[] leftKeys;
            string[] rightKeys;

            if (Source.NavigationProperty != null)
            {
                leftKeys = JoinTable.SourceForeignKeyColumns;
                rightKeys = JoinTable.TargetForeignKeyColumns;
            }
            else
            {
                leftKeys = JoinTable.TargetForeignKeyColumns;
                rightKeys = JoinTable.SourceForeignKeyColumns;
            }

            callChain.AddMethodCall(EfFluentApiMethods.Map,
                new MapMethodParameter()
                    .ToTable(JoinTable.TableName)
                    .MapLeftKey(leftKeys)
                    .MapRightKey(rightKeys)
                );
        }



        private PropertySelectorParameter CreatePropertySelectorParameter(string className, NavigationPropertyCodeModel navigationProperty)
        {
            if (navigationProperty == null || string.IsNullOrEmpty(className))
            {
                return null;
            }

            return new PropertySelectorParameter(className, navigationProperty.Name);
        }

        private EfFluentApiMethods MapMultiplicityToHasAssociationFluentApiMethod(RelationshipMultiplicity multiplicity)
        {
            switch (multiplicity)
            {
                case RelationshipMultiplicity.Many:
                    return EfFluentApiMethods.HasMany;
                case RelationshipMultiplicity.One:
                    return EfFluentApiMethods.HasRequired;
                case RelationshipMultiplicity.ZeroOrOne:
                    return EfFluentApiMethods.HasOptional;
                default:
                    throw new InvalidOperationException("Invalid RelationshipMultiplicity."); //TODO: string do resourcu
            }
        }

        private EfFluentApiMethods MapMultiplicityToWithAssociationFluentApiMethod(RelationshipMultiplicity multiplicity, bool? isPrincipal = null)
        {
            switch (multiplicity)
            {
                case RelationshipMultiplicity.Many:
                    return EfFluentApiMethods.WithMany;
                case RelationshipMultiplicity.One:
                    return EfFluentApiMethods.WithRequired;
                case RelationshipMultiplicity.ZeroOrOne:
                    return EfFluentApiMethods.WithOptional;
                default:
                    throw new InvalidOperationException("Invalid RelationshipMultiplicity."); //TODO: string do resourcu
            }
        }

        private Tuple<EfFluentApiMethods, EfFluentApiMethods> FixAssociationFluentApiMethods(EfFluentApiMethods hasMethod, EfFluentApiMethods withMethod, bool isPrincipal)
        {
            if (hasMethod == EfFluentApiMethods.HasRequired && withMethod == EfFluentApiMethods.WithRequired)
            {
                if (isPrincipal)
                {
                    withMethod = EfFluentApiMethods.WithRequiredPrincipal;
                }
                else
                {
                    withMethod = EfFluentApiMethods.WithRequiredDependent;
                }
            }
            if (hasMethod == EfFluentApiMethods.HasOptional && withMethod == EfFluentApiMethods.WithOptional)
            {
                if (isPrincipal)
                {
                    withMethod = EfFluentApiMethods.WithOptionalPrincipal;
                }
                else
                {
                    withMethod = EfFluentApiMethods.WithOptionalDependent;
                }
            }

            return Tuple.Create(hasMethod, withMethod);
        }

        #endregion
    }
}
