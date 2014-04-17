using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Infrastructure.EntityFramework.EdmExtensions;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Resources;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class AddOneToManyAssociationTransformation : AddAssociationWithForeignKeyTransformation
    {
        public string[] ForeignKeyColumnNames { get; private set; }
        public ForeignKeyPropertyCodeModel[] ForeignKeyProperties { get; private set; }
        public IndexAttribute ForeignKeyIndex { get; private set; }


        public AddOneToManyAssociationTransformation(AssociationEnd principal, AssociationEnd dependent, ForeignKeyPropertyCodeModel[] foreignKeyProperties, bool? willCascadeOnDelete = null, IndexAttribute foreignKeyIndex = null)
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

        private AddOneToManyAssociationTransformation(AssociationEnd principal, AssociationEnd dependent, ForeignKeyPropertyCodeModel[] foreignKeyProperties, string[] foreignKeyColumnNames, bool? willCascadeOnDelete, IndexAttribute foreignKeyIndex)
            : base(principal, dependent, willCascadeOnDelete)
        {
            this.ForeignKeyColumnNames = foreignKeyColumnNames;
            this.ForeignKeyProperties = foreignKeyProperties;
            this.ForeignKeyIndex = foreignKeyIndex;

            if (!MultiplicityHelper.IsOneToMany(principal, dependent))
            {
                throw new ModelTransformationValidationException(Strings.Transformations_InvalidMultiplicityOneToMany);
            }

            if (principal.HasNavigationProperty && !principal.NavigationProperty.IsCollection)
            {
                throw new ModelTransformationValidationException(Strings.Transformations_InvalidManyNavigationProperty);
            }

            if (ForeignKeyColumnNames != null && foreignKeyProperties != null)
            {
                throw new ModelTransformationValidationException(Strings.Transformations_FkNamesAndFkPropsBothSpecified);
            }
        }

        protected override IEnumerable<IModelChangeOperation> CreateModelChangeOperations(IClassModelProvider modelProvider)
        {
            var baseOperations = base.CreateModelChangeOperations(modelProvider);

            var addForeignKeyPropertyOperations = new List<IModelChangeOperation>();
            if (ForeignKeyProperties != null)
            {
                var principalPks = modelProvider.GetClassCodeModel(Principal.ClassName).PrimaryKeys.ToArray();

                string indexName = null;
                if (ForeignKeyIndex != null)
                {
                    indexName = ForeignKeyIndex.GetDefaultNameIfRequired(ForeignKeyProperties.Select(p => p.Name));
                }

                for (int i = 0; i < ForeignKeyProperties.Length; i++)
                {
                    IndexAttribute index = null;
                    if (ForeignKeyIndex != null)
                    {
                        index = ForeignKeyIndex.CopyWithNameAndOrder(indexName, i);
                    }
                    var foreignKeyProperty = CreateForeignKey(ForeignKeyProperties[i], principalPks[i], index);
                    
                    addForeignKeyPropertyOperations.Add(
                            new AddPropertyToClassOperation(Dependent.ClassName, foreignKeyProperty)
                        );

                    addForeignKeyPropertyOperations.Add(
                            new AddMappingInformationOperation(new AddPropertyMapping(Dependent.ClassName, foreignKeyProperty))
                        );
                }
            }

            return baseOperations.Concat(addForeignKeyPropertyOperations);
        }

        private PrimitivePropertyCodeModel CreateForeignKey(ForeignKeyPropertyCodeModel foreignKey, PrimitivePropertyCodeModel primaryKey, IndexAttribute index = null)
        {
            bool isForeignKeyNullable = Principal.Multipticity == RelationshipMultiplicity.ZeroOrOne ? true : false;
            var foreignKeyProperty = primaryKey.MergeWith(foreignKey, isForeignKeyNullable);

            foreignKeyProperty.Column.DatabaseGeneratedOption = DatabaseGeneratedOption.None;
            foreignKeyProperty.Column.ColumnName = foreignKey.ColumnName;
            foreignKeyProperty.Column.ParameterName = null;
            foreignKeyProperty.Column.ColumnOrder = null;

            //TODO: opravdu je ok tyhle veci dat null - jeste zrevidovat
            foreignKeyProperty.Column.IsConcurrencyToken = null;
            foreignKeyProperty.Column.ColumnType = null;

            foreignKeyProperty.Column.ColumnAnnotations.Clear();
            if (index != null)
            {
                foreignKeyProperty.Column.ColumnAnnotations.Add(IndexAnnotation.AnnotationName, index);
            }
            
            return foreignKeyProperty;
        }

        protected override AddAssociationMapping CreateAssociationMappingInformation(IClassModelProvider modelProvider)
        {
            if (ForeignKeyProperties != null)
            {
                return new AddAssociationMapping(Principal, Dependent)
                {
                    ForeignKeyProperties = ForeignKeyProperties.Select(p => p.Name).ToArray(),
                    WillCascadeOnDelete = WillCascadeOnDelete
                };
            }
            else
            {
                if (ForeignKeyColumnNames == null)
                {
                    ForeignKeyColumnNames = AddAssociationWithForeignKeyTransformation.GetUniquifiedDefaultForeignKeyColumnNames(
                        Principal,
                        Dependent,
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
            string[] foreignKeyPropertiesNames = null;
            if (ForeignKeyProperties != null)
            {
                foreignKeyPropertiesNames = ForeignKeyProperties.Select(p => p.Name).ToArray();
            }

            return new RemoveOneToManyAssociationTransformation(Principal.ToSimpleAssociationEnd(), Dependent.ToSimpleAssociationEnd(), foreignKeyPropertiesNames);
        }
    }
}
