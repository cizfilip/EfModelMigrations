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
        public AddOneToOneForeignKeyAssociationTransformation(AssociationCodeModel model)
            :base(model)
        {
            if (!Model.IsOneToOne())
            {
                throw new ModelTransformationValidationException(Strings.Transformations_InvalidMultiplicityOneToOneFk);
            }
        }

        protected override AddAssociationMapping CreateAssociationMapping(IClassModelProvider modelProvider)
        {
            var principalCodeClass = modelProvider.GetClassCodeModel(Model.Principal.ClassName);

            

            //TODO: udelat z tohohle precondition! - a pak pouzivat i v OneToMany a ManyToMany
            //if (foreignKeyColumnNames != null && principalCodeClass.PrimaryKeys.Count() != foreignKeyColumnNames.Count())
            //{
            //    throw new ModelTransformationValidationException("Supplied foreign key column names for one to one association are invalid."); //TODO: string do resourcu
            //}

            var foreignKeyColumnNames = Model.GetForeignKeyColumnNames();
            if (foreignKeyColumnNames == null)
            {
                foreignKeyColumnNames = AddAssociationWithForeignKeyTransformation.GetUniquifiedDefaultForeignKeyColumnNames(
                        Model.Principal,
                        Model.Dependent,
                        modelProvider.GetClassCodeModel(Model.Principal.ClassName),
                        modelProvider.GetClassCodeModel(Model.Dependent.ClassName));

                Model.AddInformation(AssociationInfo.CreateForeignKeyColumnNames(foreignKeyColumnNames));
            }

            return base.CreateAssociationMapping(modelProvider);
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveOneToOneForeignKeyAssociationTransformation(Model.Principal.ToSimpleAssociationEnd(), Model.Dependent.ToSimpleAssociationEnd());
        }
        
        
    }
}
