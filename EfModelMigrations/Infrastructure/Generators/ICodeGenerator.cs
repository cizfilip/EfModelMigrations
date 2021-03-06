﻿using EfModelMigrations.Infrastructure.CodeModel;
using System.Collections.Generic;

namespace EfModelMigrations.Infrastructure.Generators
{
    public interface ICodeGenerator
    {
        string GenerateEmptyClass(string name, string @namespace,
            CodeModelVisibility? visibility, string baseType,
            IEnumerable<string> implementedInterfaces);
        string GenerateProperty(PropertyCodeModel propertyModel, out string propertyName);

        string GenerateDbSetProperty(string className, out string dbSetPropertyName);

        string GetFileExtensions();

        IMappingInformationsGenerator MappingGenerator { get; }
    }
}
