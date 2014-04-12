using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations;
using EfModelMigrations.Operations;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    //TODO: je destructive nebo ne?? stejne jako remove associations nejspis???
    public class MergeClassesTransformation : TransformationWithInverse
    {
        private RemoveClassTransformation removeClass;
        private RemoveOneToOneForeignKeyAssociationTransformation removeAssociation;
        private IEnumerable<AddPropertyTransformation> addProperties;
        private IEnumerable<RemovePropertyTransformation> removeProperties; //Only for Db migration operations

        public SimpleAssociationEnd Principal { get; private set; }
        public SimpleAssociationEnd Dependent { get; private set; }
        public string[] PropertiesToMerge { get; private set; }

        public MergeClassesTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, string[] propertiesToMerge, ExtractClassTransformation inverse)
            : base(inverse)
        {
            Check.NotNull(principal, "principal");
            Check.NotNull(dependent, "dependent");
            Check.NotNullOrEmpty(propertiesToMerge, "propertiesToMerge");

            this.Principal = principal;
            this.Dependent = dependent;
            this.PropertiesToMerge = propertiesToMerge;

            //TODO: stringy do resourců
            if (!principal.HasNavigationPropertyName && !dependent.HasNavigationPropertyName)
            {
                throw new ModelTransformationValidationException("You must specify at least one navigation property for merge classes.");
            }
        }

        public MergeClassesTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, string[] propertiesToMerge)
            : this(principal, dependent, propertiesToMerge, null)
        {
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            this.removeClass = new RemoveClassTransformation(Dependent.ClassName);
            this.removeAssociation = new RemoveOneToOneForeignKeyAssociationTransformation(Principal, Dependent);
            this.addProperties = modelProvider.GetClassCodeModel(Dependent.ClassName).Properties.Where(p => PropertiesToMerge.Contains(p.Name))
                .Select(p => new AddPropertyTransformation(Principal.ClassName, p));
            this.removeProperties = PropertiesToMerge.Select(p => new RemovePropertyTransformation(Dependent.ClassName, p));

            var operations = new List<IModelChangeOperation>();

            operations.AddRange(addProperties.SelectMany(pt => pt.GetModelChangeOperations(modelProvider)));
            operations.AddRange(removeAssociation.GetModelChangeOperations(modelProvider));
            operations.AddRange(removeClass.GetModelChangeOperations(modelProvider));

            return operations;
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            var operations = new List<MigrationOperation>();
            
            //add columns
            var addColumnOperations = addProperties.SelectMany(pt => pt.GetDbMigrationOperations(builder));
            operations.AddRange(addColumnOperations);
            
            var dropAssociationOperations = removeAssociation.GetDbMigrationOperations(builder);
            //move data -> UpdateFrom
            var foreigKeyConstaintOp = dropAssociationOperations.OfType<DropForeignKeyOperation>().Single().Inverse as AddForeignKeyOperation;
            var from = new UpdateFromDataModel(foreigKeyConstaintOp.DependentTable, 
                removeProperties.SelectMany(pt => pt.GetDbMigrationOperations(builder))
                    .OfType<DropColumnOperation>()
                    .Select(c => c.Name)
                    .Concat(foreigKeyConstaintOp.DependentColumns)
                    .ToArray(),
                foreigKeyConstaintOp.DependentColumns.ToArray());
            var to = new UpdateFromDataModel(foreigKeyConstaintOp.PrincipalTable,
                addColumnOperations
                    .OfType<AddColumnOperation>()
                    .Select(c => c.Column.Name)
                    .Concat(foreigKeyConstaintOp.PrincipalColumns)
                    .ToArray(),
                foreigKeyConstaintOp.PrincipalColumns.ToArray());
            var inverse = new InsertFromOperation()
            {
                From = new InserFromDataModel(from.TableName, from.ColumnNames),
                To = new InserFromDataModel(to.TableName, to.ColumnNames),
            };
            operations.Add(
                    new UpdateFromOperation(inverse)
                    {
                        From = from,
                        To = to
                    }
                );

            //remove association
            operations.AddRange(dropAssociationOperations);

            //drop table
            operations.AddRange(removeClass.GetDbMigrationOperations(builder));


            return operations;
        }
        
        public override bool IsDestructiveChange
        {
            get { return true; }
        }
    }
}
