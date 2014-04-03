using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Infrastructure.EntityFramework.EdmExtensions;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    //TODO: doresit indexy
    public class AddOneToManyAssociationTransformation : AddAssociationWithForeignKeyTransformation
    {
        public string[] ForeignKeyColumnNames { get; private set; }
        public ScalarPropertyCodeModel[] ForeignKeyProperties { get; private set; }
        public IndexAttribute ForeignKeyIndex { get; private set; }


        public AddOneToManyAssociationTransformation(AssociationEnd principal, AssociationEnd dependent, ScalarPropertyCodeModel[] foreignKeyProperties, bool? willCascadeOnDelete = null, IndexAttribute foreignKeyIndex = null)
            : this(principal, dependent, foreignKeyProperties, null, willCascadeOnDelete, foreignKeyIndex)
        {
        }

        public AddOneToManyAssociationTransformation(AssociationEnd principal, AssociationEnd dependent, string[] foreignKeyColumnNames, bool? willCascadeOnDelete = null, IndexAttribute foreignKeyIndex = null)
            : this(principal, dependent, null, foreignKeyColumnNames, willCascadeOnDelete, foreignKeyIndex)
        {
        }

        public AddOneToManyAssociationTransformation(AssociationEnd principal, AssociationEnd dependent, bool? willCascadeOnDelete = null, IndexAttribute foreignKeyIndex = null)
            : this(principal, dependent, null, null, willCascadeOnDelete, foreignKeyIndex)
        {
        }

        private AddOneToManyAssociationTransformation(AssociationEnd principal, AssociationEnd dependent, ScalarPropertyCodeModel[] foreignKeyProperties, string[] foreignKeyColumnNames, bool? willCascadeOnDelete, IndexAttribute foreignKeyIndex)
            :base(principal, dependent, willCascadeOnDelete)
        {
            this.ForeignKeyColumnNames = foreignKeyColumnNames;
            this.ForeignKeyProperties = foreignKeyProperties;
            this.ForeignKeyIndex = foreignKeyIndex;

            //TODO: stringy do resourců
            if (!((principal.Multipticity == RelationshipMultiplicity.One || principal.Multipticity == RelationshipMultiplicity.ZeroOrOne ) 
                && dependent.Multipticity == RelationshipMultiplicity.Many))
            {
                throw new ModelTransformationValidationException("Invalid association multiplicity for one to many association.");
            }

            if (principal.HasNavigationProperty && !principal.NavigationProperty.IsCollection)
            {
                throw new ModelTransformationValidationException("Principal navigation property in one to many association must have IsCollection set to true.");
            }

            if (ForeignKeyColumnNames != null && foreignKeyProperties != null)
            {
                throw new ModelTransformationValidationException("Foreign key properties and foreign key column names are both specified. You can specify only foreign key properties or foreign key column names not both.");
            }
        }

        protected override IEnumerable<IModelChangeOperation> CreateModelChangeOperations(IClassModelProvider modelProvider)
        {
            var baseOperations = base.CreateModelChangeOperations(modelProvider);

            var addForeignKeyPropertyOperations = new List<IModelChangeOperation>();
            if (ForeignKeyProperties != null)
            {
                for (int i = 0; i < ForeignKeyProperties.Length; i++)
                {
                    var foreignKeyProperty = ForeignKeyProperties[i];

                    addForeignKeyPropertyOperations.Add(
                            new AddPropertyToClassOperation(Dependent.ClassName, foreignKeyProperty)
                        );

                    var propertyMapping = new AddPropertyMapping(foreignKeyProperty);
                    if(ForeignKeyIndex != null)
                    {
                        propertyMapping.Index = ForeignKeyIndex.CopyWithNameAndOrder(ForeignKeyIndex.Name, i);
                    }

                    addForeignKeyPropertyOperations.Add(
                            new AddMappingInformationOperation(propertyMapping)
                        );
                }
            }

            return baseOperations.Concat(addForeignKeyPropertyOperations);
        }

        protected override AddAssociationMapping CreateAssociationMappingInformation(IClassModelProvider modelProvider)
        {
            if(ForeignKeyProperties != null)
            {
                return new AddAssociationMapping(Principal, Dependent)
                {
                    ForeignKeyProperties = ForeignKeyProperties.Select(p => p.Name).ToArray(),
                    WillCascadeOnDelete = WillCascadeOnDelete
                };
            }
            else
            {
                if(ForeignKeyColumnNames == null)
                {
                    ForeignKeyColumnNames = GetDefaultForeignKeyColumnNames(
                        modelProvider.GetClassCodeModel(Principal.ClassName),
                        modelProvider.GetClassCodeModel(Dependent.ClassName));
                }

                return new AddAssociationMapping(Principal, Dependent)
                {
                    ForeignKeyColumnNames = ForeignKeyColumnNames,
                    ForeignKeyIndex = ForeignKeyIndex,
                    WillCascadeOnDelete = WillCascadeOnDelete
                };
            }
        }
        
        public override ModelTransformation Inverse()
        {
            return null;
        } 
       
    }
}
