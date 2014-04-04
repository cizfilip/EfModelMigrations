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
                throw new EfModelException(string.Format("Cannot find join table for association from {0} to {1} in entity framework metadata", from.ClassName, to.ClassName), e); //TODO: string do resourcu
            }
        }

        public AssociationType GetStorageAssociationTypeForAssociation(AssociationEnd from, AssociationEnd to)
        {
            Check.NotNull(from, "from");
            Check.NotNull(to, "to");

            return GetStorageAssociationTypeForAssociation(from.ToSimpleAssociationEnd(), to.ToSimpleAssociationEnd());
        }

        public AssociationType GetStorageAssociationTypeForAssociation(SimpleAssociationEnd from, SimpleAssociationEnd to)
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
                throw new EfModelException(string.Format("Cannot find association from {0} to {1} in entity framework metadata", from.ClassName, to.ClassName), e); //TODO: string do resourcu
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
                throw new EfModelException(string.Format("Cannot find class {0} in entity framework metadata", className), e); //TODO: string do resourcu
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
                throw new EfModelException(string.Format("Cannot find class {0} in entity framework metadata", className), e); //TODO: string do resourcu
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
                throw new EfModelException(string.Format("Cannot find column for property {0} in class {1}", propertyName, className), e); //TODO: string do resourcu
            }
        }

        public ColumnModel GetColumnModelForProperty(string className, string propertyName)
        {
            Check.NotEmpty(className, "className");
            Check.NotEmpty(propertyName, "propertyName");

            try
            {
                var storeProperty = Metadata.GetScalarPropertyMappingForProperty(className, propertyName)
                    .Column;

                return storeProperty.ToColumnModel(Metadata.ProviderManifest);
            }
            catch (Exception e)
            {
                throw new EfModelException(string.Format("Cannot get ColumnModel for property {0} in class {1}", propertyName, className), e); //TODO: string do resourcu
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
