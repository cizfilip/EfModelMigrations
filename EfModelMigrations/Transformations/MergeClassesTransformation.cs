using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
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

        public SimpleAssociationEnd Principal { get; private set; }
        public SimpleAssociationEnd Dependent { get; private set; }
        public string[] PropertiesToMerge { get; private set; }

        public MergeClassesTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, string[] propertiesToMerge, ModelTransformation inverse)
            : base(inverse)
        {
            Check.NotNull(principal, "principal");
            Check.NotNull(dependent, "dependent");
            Check.NotNullOrEmpty(propertiesToMerge, "propertiesToMerge");

            this.Principal = principal;
            this.Dependent = dependent;
            this.PropertiesToMerge = propertiesToMerge;
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

            //TODO: move data
            //var foreigKeyConstaintOp = associationOperations.OfType<AddForeignKeyOperation>().Single();
            //var from = new InsertDataModel(foreigKeyConstaintOp.PrincipalTable, dropColumnOperations
            //    .OfType<DropColumnOperation>()
            //    .Select(c => c.Name)
            //    .Concat(foreigKeyConstaintOp.PrincipalColumns)
            //    .ToArray());
            //var to = new InsertDataModel(foreigKeyConstaintOp.DependentTable, addColumnOperations
            //    .OfType<AddColumnOperation>()
            //    .Select(c => c.Column.Name)
            //    .Concat(foreigKeyConstaintOp.DependentColumns)
            //    .ToArray());
            //operations.Add(
            //        new InsertFromOperation()
            //        {
            //            From = from,
            //            To = to
            //        }
            //    );

            //remove association
            var associationOperations = removeAssociation.GetDbMigrationOperations(builder);
            operations.AddRange(associationOperations);

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
