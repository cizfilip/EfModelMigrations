using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class RemoveAssociationTransformation : TransformationWithInverse
    {
        public SimpleAssociationEnd Source { get; private set; }
        public SimpleAssociationEnd Target { get; private set; }

        public RemoveAssociationTransformation(SimpleAssociationEnd source, SimpleAssociationEnd target, ModelTransformation inverse) : base(inverse)
        {
            this.Source = source;
            this.Target = target;
        }

        public RemoveAssociationTransformation(SimpleAssociationEnd source, SimpleAssociationEnd target)
            : this(source, target, null)
        {
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            if (!string.IsNullOrEmpty(Source.NavigationPropertyName))
            {
                yield return new RemovePropertyFromClassOperation(Source.ClassName, Source.NavigationPropertyName);
            }

            if (!string.IsNullOrEmpty(Target.NavigationPropertyName))
            {
                yield return new RemovePropertyFromClassOperation(Target.ClassName, Target.NavigationPropertyName);
            }

            yield return new RemoveMappingInformationOperation(new RemoveAssociationMapping(Source, Target));
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            throw new NotImplementedException();
        }

        public override bool IsDestructiveChange
        {
            get { return true; }
        }
    }
}
