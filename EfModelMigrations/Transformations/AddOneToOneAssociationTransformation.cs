using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;

namespace EfModelMigrations.Transformations
{

    public class AddOneToOneAssociationTransformation : ModelTransformation
    {
        public AssociationMemberInfo Principal { get; private set; }
        public AssociationMemberInfo Dependent { get; private set; }


        //TODO: dodelat pridavani FK do trid
        //public bool IncludeForeignKeyInDependent { get; set; }


        public AddOneToOneAssociationTransformation(AssociationMemberInfo principal, AssociationMemberInfo dependent)
        {
            this.Principal = principal;
            this.Dependent = dependent;

            //TODO: validace - musi byt specifikovana alespon jedna navigacni property!
        }

       

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            if(Principal.NavigationProperty != null)
            {
                yield return new AddPropertyToClassOperation(Principal.ClassName, Principal.NavigationProperty.ToPropertyCodeModel());
            }

            if(Dependent.NavigationProperty != null)
            {
                yield return new AddPropertyToClassOperation(Dependent.ClassName, Dependent.NavigationProperty.ToPropertyCodeModel());
            }

            yield return new AddMappingInformationOperation(new OneToOneAssociationInfo(Principal, Dependent));
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            if(Principal.IsRequired || Dependent.IsRequired) // 1:1, 0:1, 1:0 = PK will be also FK
            {
                return builder.OneToOneRelationOperations(Principal.ClassName, Dependent.ClassName);
            }
            else //both optional = 1:n with unique index
            {
                throw new NotSupportedException("One to One association with both ends optional not supported yet.");
            }
        }
        
        public override ModelTransformation Inverse()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class AssociationMemberInfo
    {
        public AssociationMemberInfo(string className, bool isRequired, NavigationPropertyCodeModel navigationProperty)
        {
            this.ClassName = className;
            this.IsRequired = IsRequired;
            this.NavigationProperty = navigationProperty;
        }

        public string ClassName { get; private set; }
        public bool IsRequired { get; private set; }
        public NavigationPropertyCodeModel NavigationProperty { get; private set; }
    }

}
