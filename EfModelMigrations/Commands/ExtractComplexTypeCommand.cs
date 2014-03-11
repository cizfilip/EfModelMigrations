﻿using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    
    public class ExtractComplexTypeCommand : ModelMigrationsCommand
    {
        private string className;
        private string complexTypeName;
        private IEnumerable<string> propertiesToExtract;
        private NavigationProperty navigationProperty;

        //TODO: Dat stringy vyjimek do resourcu
        public ExtractComplexTypeCommand(string className, string complexTypeName, string[] properties)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ModelMigrationsException("Class from extract not specified.");
            }
            if (string.IsNullOrWhiteSpace(complexTypeName))
            {
                throw new ModelMigrationsException("Complex type name not specified.");
            }
            if (properties == null || properties.Length == 0)
            {
                throw new ModelMigrationsException("No properties to extract.");
            }

            //TODO: parsovat i dalsi veci az budou hotovz lepsi parametry z powershellu
            this.className = className;
            this.complexTypeName = complexTypeName;
            this.propertiesToExtract = properties;
            this.navigationProperty = NavigationProperty.Default(complexTypeName);
        }

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            yield return new ExtractComplexTypeTransformation(className, complexTypeName, propertiesToExtract, navigationProperty);
        }

        public override string GetMigrationName()
        {
            return "ExtractComplexType" + complexTypeName + "From" + className;
        }
    }
   
}
