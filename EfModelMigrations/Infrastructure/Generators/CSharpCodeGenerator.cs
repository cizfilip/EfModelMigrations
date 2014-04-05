using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.Generators.Templates;
using Microsoft.CSharp.RuntimeBinder;
using System.Data.Entity.Core.Metadata.Edm;
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
            bool mayBeNullable = false;
            string returnType;

            switch (property.Type)
            {
                case PrimitiveTypeKind.Binary:
                    returnType = "byte[]";
                    break;
                case PrimitiveTypeKind.Boolean:
                    returnType = "bool";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.Byte:
                    returnType = "byte";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.DateTime:
                    returnType = "DateTime";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.Time:
                    returnType = "TimeSpan";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.DateTimeOffset:
                    returnType = "DateTimeOffset";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.Decimal:
                    returnType = "decimal";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.Double:
                    returnType = "double";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.Geography:
                case PrimitiveTypeKind.GeographyPoint:
                case PrimitiveTypeKind.GeographyLineString:
                case PrimitiveTypeKind.GeographyPolygon:
                case PrimitiveTypeKind.GeographyMultiPoint:
                case PrimitiveTypeKind.GeographyMultiLineString:
                case PrimitiveTypeKind.GeographyMultiPolygon:
                case PrimitiveTypeKind.GeographyCollection:
                    returnType = "System.Data.Entity.Spatial.DbGeography"; //TODO: mozna ne fullname ale pak musi byt using ve tride....
                    break;
                case PrimitiveTypeKind.Geometry:
                case PrimitiveTypeKind.GeometryPoint:
                case PrimitiveTypeKind.GeometryLineString:
                case PrimitiveTypeKind.GeometryPolygon:
                case PrimitiveTypeKind.GeometryMultiPoint:
                case PrimitiveTypeKind.GeometryMultiLineString:
                case PrimitiveTypeKind.GeometryMultiPolygon:
                case PrimitiveTypeKind.GeometryCollection:
                    returnType = "System.Data.Entity.Spatial.DbGeometry"; //TODO: mozna ne fullname ale pak musi byt using ve tride....
                    break;
                case PrimitiveTypeKind.Guid:
                    returnType = "Guid";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.Single:
                    returnType = "float";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.SByte:
                    returnType = "sbyte";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.Int16:
                    returnType = "short";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.Int32:
                    returnType = "int";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.Int64:
                    returnType = "long";
                    mayBeNullable = true;
                    break;
                case PrimitiveTypeKind.String:
                    returnType = "string";
                    break;
                default:
                    throw new InvalidOperationException("Invalid PrimitiveTypeKind."); //TODO: string do resourcu
            }

            if (mayBeNullable && property.IsTypeNullable)
            {
                returnType = returnType + "?";
            }

            return returnType;
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

        protected virtual string GetPropertyTypeAsString(EnumPropertyCodeModel property)
        {
            return property.IsTypeNullable ? property.EnumType + "?" : property.EnumType;
        }

        


        
    }
}
