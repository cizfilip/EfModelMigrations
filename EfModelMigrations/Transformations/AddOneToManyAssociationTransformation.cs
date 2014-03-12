using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
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
    public class AddOneToManyAssociationTransformation : AddAssociationWithCascadeDeleteTransformation
    {
        public string[] ForeignKeyColumnNames { get; private set; }
        public ScalarProperty[] ForeignKeyProperties { get; private set; }

        public bool IsDependentRequired { get; private set; }


        public AddOneToManyAssociationTransformation(AssociationMemberInfo principal, AssociationMemberInfo dependent, ScalarProperty[] foreignKeyProperties, bool isDependentRequired, bool willCascadeOnDelete)
            : this(principal, dependent, foreignKeyProperties, null, isDependentRequired, willCascadeOnDelete)
        {
        }

        public AddOneToManyAssociationTransformation(AssociationMemberInfo principal, AssociationMemberInfo dependent, string[] foreignKeyColumnNames, bool isDependentRequired, bool willCascadeOnDelete)
            : this(principal, dependent, null, foreignKeyColumnNames, isDependentRequired, willCascadeOnDelete)
        {
        }

        private AddOneToManyAssociationTransformation(AssociationMemberInfo principal, AssociationMemberInfo dependent, ScalarProperty[] foreignKeyProperties, string[] foreignKeyColumnNames, bool isDependentRequired, bool willCascadeOnDelete)
            :base(principal, dependent, willCascadeOnDelete)
        {
            this.IsDependentRequired = isDependentRequired;
            this.ForeignKeyColumnNames = foreignKeyColumnNames;
            this.ForeignKeyProperties = foreignKeyProperties;

            //TODO: stringy do resourců
            if (principal.NavigationProperty != null && !principal.NavigationProperty.IsCollection)
            {
                throw new ModelTransformationValidationException("Principal navigation property in one to many association must be have IsCollection set to true.");
            }

            if (ForeignKeyColumnNames == null && foreignKeyProperties == null)
            {
                throw new ModelTransformationValidationException("Foreign key properties/columns not specified. You must supply foreign key properties or foreign key column names.");
            }

            if (ForeignKeyColumnNames != null && foreignKeyProperties != null)
            {
                throw new ModelTransformationValidationException("Foreign key properties and foreign key column names are both specified. You can specify only foreign key properties or foreign key column names not both.");
            }
        }

        protected override IEnumerable<IModelChangeOperation> CreateModelChangeOperations()
        {
            var baseOperations = base.CreateModelChangeOperations();

            var addForeignKeyPropertyOperations = new List<IModelChangeOperation>();

            if (ForeignKeyProperties != null)
            {
                foreach (var foreignKeyProperty in ForeignKeyProperties)
                {
                    addForeignKeyPropertyOperations.Add(
                            new AddPropertyToClassOperation(Dependent.ClassName, foreignKeyProperty)
                        );
                }
            }

            return baseOperations.Concat(addForeignKeyPropertyOperations);
        }

        protected override AssociationInfo CreateMappingInformation()
        {
            if(ForeignKeyColumnNames != null)
            {
                return new OneToManyWithForeignKeyColumnsAssociationInfo(Principal, Dependent, IsDependentRequired, ForeignKeyColumnNames, WillCascadeOnDelete);
            }
            else
            {
                return new OneToManyWithForeignKeyPropertiesAssociationInfo(Principal, 
                    Dependent, 
                    IsDependentRequired,
                    GetForeignKeyPropertyNames(), 
                    WillCascadeOnDelete);
            }
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            //TODO: bacha pokud delam 1:n a pridavam do trid explicitni cizi klice zde predavam nazvy properties nikoliv nazvy sloupcu (dle defaultnich ef konvenci je to jedno ale pokud by to bylo jinak tak to nefunguje)
            if (ForeignKeyColumnNames != null)
            {
                return builder.OneToManyRelationOperations(Principal.ClassName, Dependent.ClassName, IsDependentRequired, ForeignKeyColumnNames, WillCascadeOnDelete);
            }
            else
            {
                return builder.OneToManyRelationOperations(Principal.ClassName, 
                    Dependent.ClassName, 
                    IsDependentRequired,
                    GetForeignKeyPropertyNames(), 
                    WillCascadeOnDelete);
            }
        }
        
        public override ModelTransformation Inverse()
        {
            return null;
        } 
       

        private string[] GetForeignKeyPropertyNames()
        {
            return ForeignKeyProperties.Select(p => p.Name).ToArray();
        }
    }
}
