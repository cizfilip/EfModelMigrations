using EfModelMigrations.Exceptions;
using EfModelMigrations.Extensions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.ComponentModel.DataAnnotations.Schema;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Resources;

namespace EfModelMigrations.Transformations
{
    public class AddOneToOneForeignKeyAssociationTransformation : AddAssociationWithForeignKeyTransformation
    {   
        public string[] ForeignKeyColumnNames { get; private set; }

        public AddOneToOneForeignKeyAssociationTransformation(AssociationEnd principal, AssociationEnd dependent, string[] foreignKeyColumnNames = null, bool? willCascadeOnDelete = null)
            :base(principal, dependent, willCascadeOnDelete)
        {
            this.ForeignKeyColumnNames = foreignKeyColumnNames;

            if (!MultiplicityHelper.IsOneToOne(principal, dependent))
            {
                throw new ModelTransformationValidationException(Strings.Transformations_InvalidMultiplicityOneToOneFk);
            }
        }

        protected override AddAssociationMapping CreateAssociationMappingInformation(IClassModelProvider modelProvider)
        {
            var principalCodeClass = modelProvider.GetClassCodeModel(Principal.ClassName);

            //TODO: udelat z tohohle precondition! - a pak pouzivat i v OneToMany a ManyToMany
            if(ForeignKeyColumnNames != null && principalCodeClass.PrimaryKeys.Count() != ForeignKeyColumnNames.Count())
            {
                throw new ModelTransformationValidationException("Supplied foreign key column names for one to one association are invalid."); //TODO: string do resourcu
            }

            if(ForeignKeyColumnNames == null)
            {
                ForeignKeyColumnNames = AddAssociationWithForeignKeyTransformation.GetUniquifiedDefaultForeignKeyColumnNames(
                    Principal,
                    Dependent,
                    principalCodeClass, 
                    modelProvider.GetClassCodeModel(Dependent.ClassName));
            }

            return new AddAssociationMapping(Principal, Dependent)
            {
                ForeignKeyColumnNames = ForeignKeyColumnNames,
                ForeignKeyIndex = new IndexAttribute() { IsUnique = true },
                WillCascadeOnDelete = WillCascadeOnDelete
            };
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveOneToOneForeignKeyAssociationTransformation(Principal.ToSimpleAssociationEnd(), Dependent.ToSimpleAssociationEnd());
        }
        
        
    }
}
