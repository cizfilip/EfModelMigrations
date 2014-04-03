using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.Generators.Templates;
using Microsoft.CSharp.RuntimeBinder;
using Edm = System.Data.Entity.Core.Metadata.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Entity.Infrastructure.DependencyResolution;
using EfModelMigrations.Configuration;


namespace EfModelMigrations.Infrastructure.Generators
{
    //TODO: generovat spravne nullable typy
    public class CSharpCodeGenerator : CodeGenerator
    {
        protected IPluralizationService pluralizationService;

        public CSharpCodeGenerator(CodeGeneratorDefaults defaults, IMappingInformationsGenerator mappingGenerator)
            :base(defaults, mappingGenerator)
        {
            this.pluralizationService = DbConfiguration.DependencyResolver.GetService<IPluralizationService>();
        }
        
        public override string GenerateEmptyClass(string name, string @namespace, 
            CodeModelVisibility? visibility, string baseType, 
            IEnumerable<string> implementedInterfaces)
        {
            return new ClassTemplate()
            {
                Name = name,
                Namespace = @namespace,
                Visibility = visibility ?? defaults.Class.Visibility,
                BaseType = baseType,
                ImplementedInterfaces = implementedInterfaces,
                Imports = GetDefaultImports(),
                CodeModelVisibilityMapper = CodeModelVisibilityToString
            }.TransformText();
        }

        public override string GenerateProperty(PropertyCodeModel propertyModel, out string propertyName)
        {
            propertyName = propertyModel.Name;

            return new PropertyTemplate()
            {
                Name = propertyModel.Name,
                Type = GetPropertyType(propertyModel),
                Visibility = CodeModelVisibilityToString(propertyModel.Visibility ?? defaults.Property.Visibility),
                IsVirtual = propertyModel.IsVirtual ?? defaults.Property.IsVirtual,
                IsSetterPrivate = propertyModel.IsSetterPrivate ?? defaults.Property.IsSetterPrivate
            }.TransformText();
        }

        public override string GenerateDbSetProperty(string className, out string dbSetPropertyName)
        {
            dbSetPropertyName = pluralizationService.Pluralize(className);
            return new DbSetPropertyTemplate()
            {
                GenericType = className,
                Name = dbSetPropertyName 
            }.TransformText();
        }


        public override string GetFileExtensions()
        {
            return ".cs";
        }


        protected virtual IEnumerable<string> GetDefaultImports()
        {
            yield return "System";
            yield return "System.Collections.Generic";
            yield return "System.Linq";
        }

        protected virtual string CodeModelVisibilityToString(CodeModelVisibility visibility)
        {
            switch (visibility)
            {
                case CodeModelVisibility.Public:
                    return "public";
                case CodeModelVisibility.Private:
                    return "private";
                case CodeModelVisibility.Protected:
                    return "protected";
                case CodeModelVisibility.Internal:
                    return "internal";
                case CodeModelVisibility.ProtectedInternal:
                    return "protected internal";
                default:
                    return "public";
            }
        }

        private string GetPropertyType(PropertyCodeModel property)
        {
            try
            {
                dynamic dynamicType = property;

                return GetPropertyTypeAsString(dynamicType);
            }
            catch (RuntimeBinderException e)
            {
                //TODO: string do resourcu
                throw new ModelMigrationsException(string.Format("Cannot generate property, because property type {0} is not supported", property.GetType().Name), e);
            }
        }

        protected virtual string GetPropertyTypeAsString(ScalarPropertyCodeModel property)
        {
            switch (property.Type)
            {
                case Edm.PrimitiveTypeKind.Binary:
                    return "byte[]";
                case Edm.PrimitiveTypeKind.Boolean:
                    return "bool";
                case Edm.PrimitiveTypeKind.Byte:
                    return "byte";
                case Edm.PrimitiveTypeKind.DateTime:
                    return "DateTime";
                case Edm.PrimitiveTypeKind.Time:
                    return "TimeSpan";
                case Edm.PrimitiveTypeKind.DateTimeOffset:
                    return "DateTimeOffset";
                case Edm.PrimitiveTypeKind.Decimal:
                    return "decimal";
                case Edm.PrimitiveTypeKind.Double:
                    return "double";
                case Edm.PrimitiveTypeKind.Geography:
                case Edm.PrimitiveTypeKind.GeographyPoint:
                case Edm.PrimitiveTypeKind.GeographyLineString:
                case Edm.PrimitiveTypeKind.GeographyPolygon:
                case Edm.PrimitiveTypeKind.GeographyMultiPoint:
                case Edm.PrimitiveTypeKind.GeographyMultiLineString:
                case Edm.PrimitiveTypeKind.GeographyMultiPolygon:
                case Edm.PrimitiveTypeKind.GeographyCollection:
                    return "System.Data.Entity.Spatial.DbGeography"; //TODO: mozna ne fullname ale pak musi byt using ve tride....
                case Edm.PrimitiveTypeKind.Geometry:
                case Edm.PrimitiveTypeKind.GeometryPoint:
                case Edm.PrimitiveTypeKind.GeometryLineString:
                case Edm.PrimitiveTypeKind.GeometryPolygon:
                case Edm.PrimitiveTypeKind.GeometryMultiPoint:
                case Edm.PrimitiveTypeKind.GeometryMultiLineString:
                case Edm.PrimitiveTypeKind.GeometryMultiPolygon:
                case Edm.PrimitiveTypeKind.GeometryCollection:
                    return "System.Data.Entity.Spatial.DbGeometry";
                case Edm.PrimitiveTypeKind.Guid:
                    return "Guid";
                case Edm.PrimitiveTypeKind.Single:
                    return "float";
                case Edm.PrimitiveTypeKind.SByte:
                    return "sbyte";
                case Edm.PrimitiveTypeKind.Int16:
                    return "short";
                case Edm.PrimitiveTypeKind.Int32:
                    return "int";
                case Edm.PrimitiveTypeKind.Int64:
                    return "long";
                case Edm.PrimitiveTypeKind.String:
                    return "string";
                default:
                    throw new InvalidOperationException("Invalid PrimitiveTypeKind."); //TODO: string do resourcu
            }
        }

        protected virtual string GetPropertyTypeAsString(NavigationPropertyCodeModel property)
        {
            if (property.IsCollection)
            {
                return string.Format("ICollection<{0}>", property.TargetClass);
            }
            else
            {
                return property.TargetClass;
            }
        }

        


        
    }
}
