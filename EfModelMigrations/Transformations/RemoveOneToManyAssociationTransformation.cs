using EfModelMigrations.Infrastructure;
using EfModelMigrations.Extensions;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Operations.Mapping;

namespace EfModelMigrations.Transformations
{
    //TODO: validovat ze ForeignKeyPropertyNames jsou ok...
    public class RemoveOneToManyAssociationTransformation : RemoveAssociationWithForeignKeyTransformation
    {
        public string[] ForeignKeyPropertyNames { get; private set; }

        public RemoveOneToManyAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, string[] foreignKeyPropertyNames, ModelTransformation inverse)
            : base(principal, dependent, inverse)
        {
            this.ForeignKeyPropertyNames = foreignKeyPropertyNames;
        }

        public RemoveOneToManyAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, ModelTransformation inverse)
            : this(principal, dependent, null, inverse)
        {
        }

        public RemoveOneToManyAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, string[] foreignKeyPropertyNames)
            : this(principal, dependent, foreignKeyPropertyNames, null)
        {
        }

        public RemoveOneToManyAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent)
            : this(principal, dependent, null, null)
        {
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            var baseOperations = base.GetModelChangeOperations(modelProvider);

            var removeForeignKeyPropertyOperations = new List<IModelChangeOperation>();
            if(ForeignKeyPropertyNames != null)
            {
                foreach (var foreignKeyPropertyName in ForeignKeyPropertyNames)
                {
                    removeForeignKeyPropertyOperations.Add(
                        new RemoveMappingInformationOperation(new RemovePropertyMapping(Dependent.ClassName, foreignKeyPropertyName))
                    );
                    removeForeignKeyPropertyOperations.Add(
                        new RemovePropertyFromClassOperation(Dependent.ClassName, foreignKeyPropertyName)
                    );
                }
            }
          
            return baseOperations.Concat(removeForeignKeyPropertyOperations);
        }
    }
}
