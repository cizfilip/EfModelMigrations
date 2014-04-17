using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using EfModelMigrations.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure.Annotations;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Transformations.Model;
using EfModelMigrations.Resources;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public sealed class EfModel
    {
        public EfModelMetadata Metadata { get; private set; }

        public EfModel(string edmx)
        {
            this.Metadata = EfModelMetadata.Load(edmx);
        }

        internal EfModel(EfModelMetadata metadata)
        {
            this.Metadata = metadata;
        }

        public IEnumerable<AssociationSet> GetAssociationSetsForEntitySet(EntitySet entitySet)
        {
            Check.NotNull(entitySet, "entitySet");

            return entitySet.EntityContainer.AssociationSets.Where(a => a.AssociationSetEnds.Any(e => e.EntitySet.Equals(entitySet)));
        }

        public EntitySet GetStoreEntitySetJoinTableForManyToMany(AssociationEnd from, AssociationEnd to)
        {
            Check.NotNull(from, "from");
            Check.NotNull(to, "to");

            return GetStoreEntitySetJoinTableForManyToMany(from.ToSimpleAssociationEnd(), to.ToSimpleAssociationEnd());
        }

        public EntitySet GetStoreEntitySetJoinTableForManyToMany(SimpleAssociationEnd from, SimpleAssociationEnd to)
        {
            Check.NotNull(from, "from");
            Check.NotNull(to, "to");

            try 
            {
                if (from.HasNavigationPropertyName)
                {
                    return GetStoreEntitySetJoinTableForManyToManyFromAssociationEnd(from);
                }
                else
                {
                    return GetStoreEntitySetJoinTableForManyToManyFromAssociationEnd(to);
                }
            }
            catch (Exception e)
            {
                throw new EfModelException(Strings.EfModel_CannotFindJoinTable(from.ClassName, to.ClassName), e);
            }
        }

        public AssociationType GetStoreAssociationTypeForAssociation(AssociationEnd from, AssociationEnd to)
        {
            Check.NotNull(from, "from");
            Check.NotNull(to, "to");

            return GetStoreAssociationTypeForAssociation(from.ToSimpleAssociationEnd(), to.ToSimpleAssociationEnd());
        }

        public AssociationType GetStoreAssociationTypeForAssociation(SimpleAssociationEnd from, SimpleAssociationEnd to)
        {
            Check.NotNull(from, "from");
            Check.NotNull(to, "to");

            try
            {
                if (from.HasNavigationPropertyName)
                {
                    return GetStoreAssociationTypeFromAssociationEnd(from);
                }
                else
                {
                    return GetStoreAssociationTypeFromAssociationEnd(to);
                }
            }
            catch (Exception e)
            {
                throw new EfModelException(Strings.EfModel_CannotFindAssociation(from.ClassName, to.ClassName), e);
            }
            
        }

        public EntitySet GetStoreEntitySetForClass(string className)
        {
            Check.NotEmpty(className, "className");
            try
            {
                return Metadata.GetStoreEntitySetForClass(className);
            }
            catch (Exception e)
            {
                throw new EfModelException(Strings.EfModel_CannotFindClass(className), e);
            }
        }

        public EntityType GetStoreEntityTypeForClass(string className)
        {
            Check.NotEmpty(className, "className");
            try
            {
                return Metadata.GetStoreEntitySetForClass(className).ElementType;
            }
            catch (Exception e)
            {
                throw new EfModelException(Strings.EfModel_CannotFindClass(className), e);
            }
        }


        public EdmProperty GetStoreColumnForProperty(string className, string propertyName)
        {
            Check.NotEmpty(className, "className");
            Check.NotEmpty(propertyName, "propertyName");

            try
            {
                return Metadata.GetScalarPropertyMappingForProperty(className, propertyName)
                    .Column;
            }
            catch (Exception e)
            {
                throw new EfModelException(Strings.EfModel_CannotFindColumn(propertyName, className), e);
            }
        }
       

        //Private methods
        private AssociationType GetStoreAssociationTypeFromAssociationEnd(SimpleAssociationEnd associationEnd)
        {
            var associationName = GetAssociationNameFromAssociationEnd(associationEnd);

            return Metadata.StoreAssociatonTypes.Single(at => at.Name.EqualsOrdinal(associationName));
        }

        private EntitySet GetStoreEntitySetJoinTableForManyToManyFromAssociationEnd(SimpleAssociationEnd associationEnd)
        {
            var associationName = GetAssociationNameFromAssociationEnd(associationEnd);

            return Metadata.EntityContainerMapping.AssociationSetMappings
                .Where(asm => asm.AssociationTypeMapping.AssociationType.Name.EqualsOrdinal(associationName))
                .Single()
                .StoreEntitySet;
        }

        private string GetAssociationNameFromAssociationEnd(SimpleAssociationEnd associationEnd)
        {
            return GetNavigationPropertyFromAssociationEnd(associationEnd)
                    .RelationshipType
                    .Name;
        }

        private NavigationProperty GetNavigationPropertyFromAssociationEnd(SimpleAssociationEnd associationEnd)
        {
            return Metadata.GetEntityTypeForClass(associationEnd.ClassName)
                    .NavigationProperties
                    .Single(np => np.Name.EqualsOrdinal(associationEnd.NavigationPropertyName));
        }
    }
}
