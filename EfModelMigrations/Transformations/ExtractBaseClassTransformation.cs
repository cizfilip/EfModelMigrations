using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Transformations.Model;
using EfModelMigrations.Transformations.Preconditions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    //TODO: odstranit
    public class ExtractBaseClassTransformation : ModelTransformation
    {
        public string[] FromClasses { get; private set; }
        public ClassModel NewBaseClass { get; private set; }
        public string[] PropertiesToExtract { get; private set; }
        
        public ExtractBaseClassTransformation(string[] fromClasses, ClassModel newBaseClass, string[] propertiesToExtract)
        {
            Check.NotNullOrEmpty(fromClasses, "fromClasses");
            Check.NotNull(newBaseClass, "newBaseClass");
            Check.NotNullOrEmpty(propertiesToExtract, "propertiesToExtract");

            this.FromClasses = fromClasses;
            this.NewBaseClass = newBaseClass;
            this.PropertiesToExtract = propertiesToExtract;
        }


        public override IEnumerable<ModelTransformationPrecondition> GetPreconditions()
        {

            //Preconditions: 
            //- FromClasses does not have base
            //- NewBaseClass does not exists in model
            //- All classes from FromClasses does have all properties to extract
            
            return base.GetPreconditions();
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            //create new base class--- (force pk is not identity for simplification)
            //add properties to new base class
            //set base on fromclasses
            //remove properties from fromclasses

            //handle mapping informations
            return base.GetModelChangeOperations(modelProvider);
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            return base.GetDbMigrationOperations(builder);
        }


        public override ModelTransformation Inverse()
        {
            return null;
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
