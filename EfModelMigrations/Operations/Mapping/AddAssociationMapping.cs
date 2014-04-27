using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations.Model;
using EfModelMigrations.Infrastructure.EntityFramework.EdmExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Resources;

namespace EfModelMigrations.Operations.Mapping
{
    public class AddAssociationMapping : IAddMappingInformation
    {
        public AssociationCodeModel Model { get; private set; }

        public AddAssociationMapping(AssociationCodeModel model)
        {
            Check.NotNull(model, "model");

            this.Model = model;
        }


        public EfFluentApiCallChain BuildEfFluentApiCallChain()
        {
            EfFluentApiCallChain callChain;

            if (Model.Principal.HasNavigationProperty) //Navigation property on Source
            {
                var methods = GetFluentApiMethodsStartingFromSource();
                callChain = new EfFluentApiCallChain(Model.Principal.ClassName)
                    .AddMethodCall(methods.Item1, CreatePropertySelectorParameter(Model.Principal.ClassName, Model.Principal.NavigationProperty))
                    .AddMethodCall(methods.Item2, CreatePropertySelectorParameter(Model.Dependent.ClassName, Model.Dependent.NavigationProperty));
            }
            else //Navigation property on Target
            {
                var methods = GetFluentApiMethodsStartingFromTarget();
                callChain = new EfFluentApiCallChain(Model.Dependent.ClassName)
                    .AddMethodCall(methods.Item1, CreatePropertySelectorParameter(Model.Dependent.ClassName, Model.Dependent.NavigationProperty))
                    .AddMethodCall(methods.Item2, CreatePropertySelectorParameter(Model.Principal.ClassName, Model.Principal.NavigationProperty));
            }

            AddAdditionalMethodCalls(callChain);            

            return callChain;
        }


        protected virtual Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromSource()
        {
            var hasMethod = MapMultiplicityToHasAssociationFluentApiMethod(Model.Dependent.Multipticity);
            var withMethod = MapMultiplicityToWithAssociationFluentApiMethod(Model.Principal.Multipticity);

            return FixAssociationFluentApiMethods(hasMethod, withMethod, isPrincipal: true);
        }
        protected virtual Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromTarget()
        {
            var hasMethod = MapMultiplicityToHasAssociationFluentApiMethod(Model.Principal.Multipticity);
            var withMethod = MapMultiplicityToWithAssociationFluentApiMethod(Model.Dependent.Multipticity);

            return FixAssociationFluentApiMethods(hasMethod, withMethod, isPrincipal: false);
        }

        
        protected virtual void AddAdditionalMethodCalls(EfFluentApiCallChain callChain)
        {
            var foreignKeyProperties = Model.GetForeignKeyProperties();
            var foreignKeyColumnNames = Model.GetForeignKeyColumnNames();
            var joinTable = Model.GetJoinTable();
            var willCascadeOnDelete = Model.GetWillCascadeOnDelete();

            if (foreignKeyProperties != null && Model.IsOneToMany())
            {
                AddHasForeignKeyMethodCall(callChain, foreignKeyProperties);
            }
            else if (foreignKeyColumnNames != null &&
                (Model.IsOneToMany() || (Model.IsOneToOne())))
            {
                AddMapForeignKeysMethodCall(callChain, foreignKeyColumnNames);
            }

            if (joinTable != null && Model.IsManyToMany())
            {
                AddMapJoinTableMethodCall(callChain, joinTable);
            }

            if (willCascadeOnDelete.HasValue && !Model.IsManyToMany())
            {
                AddCascadeOnDeleteMethodCall(callChain, willCascadeOnDelete.Value);
            }
        }

        #region Private Methods
        private void AddHasForeignKeyMethodCall(EfFluentApiCallChain callChain, ForeignKeyPropertyCodeModel[] foreignKeyProperties)
        {
            callChain.AddMethodCall(EfFluentApiMethods.HasForeignKey, new PropertySelectorParameter(Model.Dependent.ClassName, foreignKeyProperties.Select(p => p.Name).ToArray()));
        }

        private void AddMapForeignKeysMethodCall(EfFluentApiCallChain callChain, string[] foreignKeyColumnNames)
        {
            var mapMethodParameter = new MapMethodParameter().MapKey(foreignKeyColumnNames);

            var foreignKeyIndex = Model.GetForeignKeyIndex();
            if(foreignKeyIndex != null)
            {
                var indexName = foreignKeyIndex.GetDefaultNameIfRequired(foreignKeyColumnNames);
                var indexAnnotationName = IndexAnnotation.AnnotationName;

                for (int i = 0; i < foreignKeyColumnNames.Length; i++)
                {
                    mapMethodParameter.HasIndexColumnAnnotation(foreignKeyColumnNames[i], indexAnnotationName, 
                            foreignKeyIndex.CopyWithNameAndOrder(indexName, i)
                        );
                }
            }

            callChain.AddMethodCall(EfFluentApiMethods.Map, mapMethodParameter);
        }

        private void AddCascadeOnDeleteMethodCall(EfFluentApiCallChain callChain, bool willCascadeOnDelete)
        {
            callChain.AddMethodCall(EfFluentApiMethods.WillCascadeOnDelete, new ValueParameter(willCascadeOnDelete));
        }

        private void AddMapJoinTableMethodCall(EfFluentApiCallChain callChain, ManyToManyJoinTable joinTable)
        {
            string[] leftKeys;
            string[] rightKeys;

            if (Model.Principal.HasNavigationProperty)
            {
                leftKeys = joinTable.SourceForeignKeyColumns;
                rightKeys = joinTable.TargetForeignKeyColumns;
            }
            else
            {
                leftKeys = joinTable.TargetForeignKeyColumns;
                rightKeys = joinTable.SourceForeignKeyColumns;
            }

            callChain.AddMethodCall(EfFluentApiMethods.Map,
                new MapMethodParameter()
                    .ToTable(joinTable.TableName)
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
                    throw new InvalidOperationException(Strings.RelationshipMultiplicityInvalid);
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
                    throw new InvalidOperationException(Strings.RelationshipMultiplicityInvalid);
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
