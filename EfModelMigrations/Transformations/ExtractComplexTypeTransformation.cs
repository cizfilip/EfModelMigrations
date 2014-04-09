using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    //TODO: dodelat ci odstranit
    public class ExtractComplexTypeTransformation : ModelTransformation
    {
        public string ClassName { get; set; }
        public string ComplexTypeName { get; private set; }
        public IEnumerable<string> PropertiesToExtract { get; private set; }

        public NavigationPropertyCodeModel NavigationProperty { get; set; }

        public ExtractComplexTypeTransformation(string className, string complexTypeName, IEnumerable<string> propertiesToExtract, NavigationPropertyCodeModel navigationProperty)
        {
            Check.NotEmpty(className, "className");
            Check.NotEmpty(complexTypeName, "complexTypeName");
            Check.NotNullOrEmpty(propertiesToExtract, "propertiesToExtract");
            Check.NotNull(navigationProperty, "navigationProperty");
            
            this.ClassName = className;
            this.ComplexTypeName = complexTypeName;
            this.PropertiesToExtract = propertiesToExtract;
            this.NavigationProperty = navigationProperty;
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new CreateEmptyClassOperation(ComplexTypeName);


            //var classModel = modelProvider.GetClassCodeModel(ClassName);
            
            foreach (var property in PropertiesToExtract)
            {
                yield return new MovePropertyOperation(ClassName, ComplexTypeName, property);
            }            

            yield return new AddPropertyToClassOperation(ClassName, NavigationProperty);
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            foreach (var property in PropertiesToExtract)
            {
                yield return builder.RenameColumnOperation(
                        builder.NewModel.GetStoreEntitySetForClass(ClassName),
                        builder.OldModel.GetStoreColumnForProperty(ClassName, property),
                        builder.NewModel.GetStoreColumnForProperty(ClassName, property)
                    );
            }
        }

        public override ModelTransformation Inverse()
        {
            return new JoinComplexTypeTransformation(ComplexTypeName, ClassName);
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }

}
