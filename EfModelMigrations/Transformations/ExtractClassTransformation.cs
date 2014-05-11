using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.CodeModel.Builders;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations;
using EfModelMigrations.Operations;
using EfModelMigrations.Transformations.Model;
using EfModelMigrations.Transformations.Preconditions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class ExtractClassTransformation : ModelTransformation
    {
        private CreateClassTransformation createClass;
        private IEnumerable<AddPropertyTransformation> addProperties;
        private IEnumerable<RemovePropertyTransformation> removeProperties;
        private AddOneToOneForeignKeyAssociationTransformation addAssociation;

        public string FromClass { get; private set; }

        public NavigationPropertyCodeModel FromClassNavigationProperty { get; set; }

        public string[] Properties { get; private set; }

        public ClassModel NewClass { get; private set; }

        public NavigationPropertyCodeModel NewClassNavigationProperty { get; set; }

        public IEnumerable<PrimitivePropertyCodeModel> PrimaryKeys { get; set; }

        public string[] ForeignKeyColumns { get; private set; }


        public ExtractClassTransformation(string fromClass, 
            string[] properties, 
            ClassModel newClass, 
            IEnumerable<PrimitivePropertyCodeModel> primaryKeys = null, 
            NavigationPropertyCodeModel fromNavigationProp = null, 
            NavigationPropertyCodeModel newNavigationProp = null, 
            string[] foreignKeyColumns = null)
        {
            Check.NotEmpty(fromClass, "fromClass");
            Check.NotNullOrEmpty(properties, "properties");
            Check.NotNull(newClass, "newClass");

            this.FromClass = fromClass;
            this.Properties = properties;
            this.NewClass = newClass;
            this.PrimaryKeys = primaryKeys;
            this.ForeignKeyColumns = foreignKeyColumns;
            this.FromClassNavigationProperty = fromNavigationProp;
            this.NewClassNavigationProperty = newNavigationProp;
        }

        public override IEnumerable<ModelTransformationPrecondition> GetPreconditions()
        {
            yield return new ClassExistsInModelPrecondition(FromClass);
            yield return new PropertiesExistInClassPrecondition(FromClass, Properties);
            yield return new ClassNotExistsInModelPrecondition(NewClass.Name);
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            
            this.createClass = GetCreateClassTransformation(modelProvider);
            this.addProperties = modelProvider.GetClassCodeModel(FromClass).Properties.Where(p => Properties.Contains(p.Name))
                .Select(p => new AddPropertyTransformation(NewClass.Name, p));
            this.removeProperties = Properties.Select(p => new RemovePropertyTransformation(FromClass, p));
            this.addAssociation = GetAssociationTransformation(modelProvider);

            //create new class (only primary keys)
            var operations = createClass.GetModelChangeOperations(modelProvider).ToList();
            
            //add properties
            operations.AddRange(addProperties.SelectMany(pt => pt.GetModelChangeOperations(modelProvider)));
            
            //create one to one fk association
            operations.AddRange(addAssociation.GetModelChangeOperations(modelProvider));

            //remove extracted properties from class
            operations.AddRange(removeProperties.SelectMany(pt => pt.GetModelChangeOperations(modelProvider)));

            return operations;
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            var operations = new List<MigrationOperation>();
            //create table
            operations.AddRange(createClass.GetDbMigrationOperations(builder));

            //add columns - create table added all columns - this is only helper for move data operation
            var addedColumnOperations = addProperties.SelectMany(pt => pt.GetDbMigrationOperations(builder));

            //add association - without adding fk columns - these were added in create table
            var associationOperations = addAssociation.GetDbMigrationOperations(builder).Where(op => !(op is AddColumnOperation));
            operations.AddRange(associationOperations);

            var dropColumnOperations = removeProperties.SelectMany(pt => pt.GetDbMigrationOperations(builder));

            //move data -> InsertFrom
            var foreigKeyConstaintOp = associationOperations.OfType<AddForeignKeyOperation>().Single();
            var from = new InserFromDataModel(foreigKeyConstaintOp.PrincipalTable, dropColumnOperations
                .OfType<DropColumnOperation>()
                .Select(c => c.Name)
                .Concat(foreigKeyConstaintOp.PrincipalColumns)
                .ToArray());
            var to = new InserFromDataModel(foreigKeyConstaintOp.DependentTable, addedColumnOperations
                .OfType<AddColumnOperation>()
                .Select(c => c.Column.Name)
                .Concat(foreigKeyConstaintOp.DependentColumns)
                .ToArray());
            var inverse = new UpdateFromOperation()
                {
                    From = new UpdateFromDataModel(to.TableName, 
                        to.ColumnNames.Except(foreigKeyConstaintOp.DependentColumns).ToArray(), 
                        foreigKeyConstaintOp.DependentColumns.ToArray()),
                    To = new UpdateFromDataModel(from.TableName, 
                        from.ColumnNames.Except(foreigKeyConstaintOp.PrincipalColumns).ToArray(), 
                        foreigKeyConstaintOp.PrincipalColumns.ToArray())
                };
            operations.Add(
                    new InsertFromOperation(inverse)
                    {
                        From = from,
                        To = to
                    }
                );

            //drop columns
            operations.AddRange(dropColumnOperations);

            return operations;
        }

        public override ModelTransformation Inverse()
        {
            var associationModel = GetAssociationModel();
            return new MergeClassesTransformation(associationModel.Principal.ToSimpleAssociationEnd(), associationModel.Dependent.ToSimpleAssociationEnd(), Properties);
        }


        private AddOneToOneForeignKeyAssociationTransformation GetAssociationTransformation(IClassModelProvider modelProvider)
        {
            var associationModel = GetAssociationModel();
            
            if(ForeignKeyColumns == null)
            {
                ForeignKeyColumns = AddAssociationWithForeignKeyTransformation
                    .GetDefaultForeignKeyColumnNames(associationModel.Principal, associationModel.Dependent, modelProvider.GetClassCodeModel(FromClass));
            }

            associationModel.AddInformation(AssociationInfo.CreateForeignKeyColumnNames(ForeignKeyColumns));
            associationModel.AddInformation(AssociationInfo.CreateWillCascadeOnDelete(true));

            return new AddOneToOneForeignKeyAssociationTransformation(associationModel);
        }

        private AssociationCodeModel GetAssociationModel()
        {
            if(FromClassNavigationProperty == null && NewClassNavigationProperty == null)
            {
                FromClassNavigationProperty = new NavigationPropertyCodeModel(NewClass.Name);
                NewClassNavigationProperty = new NavigationPropertyCodeModel(FromClass);
            }

            var principal = new AssociationEnd(FromClass, RelationshipMultiplicity.One, FromClassNavigationProperty);
            var dependent = new AssociationEnd(NewClass.Name, RelationshipMultiplicity.One, NewClassNavigationProperty);

            return new AssociationCodeModel(principal, dependent);
        }

        private CreateClassTransformation GetCreateClassTransformation(IClassModelProvider modelProvider)
        {
            List<PrimitivePropertyCodeModel> properties = new List<PrimitivePropertyCodeModel>();
            if(PrimaryKeys == null || PrimaryKeys.Count() > 1)
            {
                PrimaryKeys = new[] { GetDefaultPrimaryKeyForNewClass() };
            }
            properties.AddRange(PrimaryKeys);
            
            return new CreateClassTransformation(NewClass, properties, PrimaryKeys.Select(p => p.Name).ToArray());
        }

        private PrimitivePropertyCodeModel GetDefaultPrimaryKeyForNewClass()
        {
            //A co kdyz property s nazvem Id bude exisotvat (tj je to jedna z extrahovanych properties)
            var prop = ((PrimitiveMappingBuilder)new PrimitivePropertyBuilder().Int()).Property;
            prop.Name = "Id";
            return prop;
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
