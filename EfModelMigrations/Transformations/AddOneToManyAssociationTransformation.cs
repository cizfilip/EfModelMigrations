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
        public AddOneToManyAssociationTransformation(AssociationCodeModel model)
            : base(model)
        {
            if (!Model.IsOneToMany())
            {
                throw new ModelTransformationValidationException(Strings.Transformations_InvalidMultiplicityOneToMany);
            }

            if (Model.Principal.HasNavigationProperty && !Model.Principal.NavigationProperty.IsCollection)
            {
                throw new ModelTransformationValidationException(Strings.Transformations_InvalidManyNavigationProperty);
            }

            if (Model.GetForeignKeyColumnNames() != null && Model.GetForeignKeyProperties() != null)
            {
                throw new ModelTransformationValidationException(Strings.Transformations_FkNamesAndFkPropsBothSpecified);
            }
        }
        
        protected override IEnumerable<IModelChangeOperation> CreateModelChangeOperations(IClassModelProvider modelProvider)
        {
            var baseOperations = base.CreateModelChangeOperations(modelProvider);

            var addForeignKeyPropertyOperations = new List<IModelChangeOperation>();
            var foreignKeyProperties = Model.GetForeignKeyProperties();
            var foreignKeyIndex = Model.GetForeignKeyIndex();

            if (foreignKeyProperties != null)
            {
                var principalPks = modelProvider.GetClassCodeModel(Model.Principal.ClassName).PrimaryKeys.ToArray();

                string indexName = null;
                if (foreignKeyIndex != null)
                {
                    indexName = foreignKeyIndex.GetDefaultNameIfRequired(foreignKeyProperties.Select(p => p.Name));
                }

                for (int i = 0; i < foreignKeyProperties.Length; i++)
                {
                    IndexAttribute index = null;
                    if (foreignKeyIndex != null)
                    {
                        index = foreignKeyIndex.CopyWithNameAndOrder(indexName, i);
                    }
                    var foreignKeyProperty = CreateForeignKey(foreignKeyProperties[i], principalPks[i], index);
                    
                    addForeignKeyPropertyOperations.Add(
                            new AddPropertyToClassOperation(Model.Dependent.ClassName, foreignKeyProperty)
                        );

                    addForeignKeyPropertyOperations.Add(
                            new AddMappingInformationOperation(new AddPropertyMapping(Model.Dependent.ClassName, foreignKeyProperty))
                        );
                }
            }

            return baseOperations.Concat(addForeignKeyPropertyOperations);
        }

        private PrimitivePropertyCodeModel CreateForeignKey(ForeignKeyPropertyCodeModel foreignKey, PrimitivePropertyCodeModel primaryKey, IndexAttribute index = null)
        {
            bool isForeignKeyNullable = Model.Principal.Multipticity == RelationshipMultiplicity.ZeroOrOne ? true : false;
            var foreignKeyProperty = primaryKey.MergeWith(foreignKey, isForeignKeyNullable);

            foreignKeyProperty.Column.DatabaseGeneratedOption = DatabaseGeneratedOption.None;
            foreignKeyProperty.Column.ColumnName = foreignKey.ColumnName;
            foreignKeyProperty.Column.ParameterName = null;
            foreignKeyProperty.Column.ColumnOrder = null;
            foreignKeyProperty.Column.IsConcurrencyToken = null;
            foreignKeyProperty.Column.IsRowVersion = null;

            foreignKeyProperty.Column.ColumnAnnotations.Clear();
            if (index != null)
            {
                foreignKeyProperty.Column.ColumnAnnotations.Add(IndexAnnotation.AnnotationName, index);
            }
            
            return foreignKeyProperty;
        }

        public override ModelTransformation Inverse()
        {
            var foreignKeyProperties = Model.GetForeignKeyProperties();

            string[] foreignKeyPropertiesNames = null;
            if (foreignKeyProperties != null)
            {
                foreignKeyPropertiesNames = foreignKeyProperties.Select(p => p.Name).ToArray();
            }

            return new RemoveOneToManyAssociationTransformation(Model.Principal.ToSimpleAssociationEnd(), Model.Dependent.ToSimpleAssociationEnd(), foreignKeyPropertiesNames);
        }
    }
}
