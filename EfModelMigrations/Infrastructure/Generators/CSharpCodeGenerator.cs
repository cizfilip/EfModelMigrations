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


namespace EfModelMigrations.Infrastructure.Generators
{
    internal class CSharpCodeGenerator : ICodeGenerator
    {
        private IMappingInformationsGenerator mappingGenerator;

        public CSharpCodeGenerator(IMappingInformationsGenerator mappingGenerator)
        {
            this.mappingGenerator = mappingGenerator;
        }

        public string GenerateEmptyClass(string name, string @namespace, 
            CodeModelVisibility visibility, string baseType, 
            IEnumerable<string> implementedInterfaces)
        {
            return new ClassTemplate()
            {
                Name = name,
                Namespace = @namespace,
                Visibility = visibility,
                BaseType = baseType,
                ImplementedInterfaces = implementedInterfaces,
                Imports = GetDefaultImports(),
                CodeModelVisibilityMapper = CodeModelVisibilityToString
            }.TransformText();
        }

        public string GenerateProperty(PropertyCodeModelBase propertyModel, out string propertyName)
        {
            propertyName = propertyModel.Name;

            return new PropertyTemplate()
            {
                PropertyModel = propertyModel,
                CodeModelVisibilityMapper = CodeModelVisibilityToString,
                CodeModelTypeMapper = CodeModelTypeToString
            }.TransformText();
        }

        public string GenerateDbSetProperty(string className, out string dbSetPropertyName)
        {
            dbSetPropertyName = className + "Set"; //TODO: asi spis pluralizovat jmeno
            return new DbSetPropertyTemplate()
            {
                GenericType = className,
                Name = dbSetPropertyName 
            }.TransformText();
        }

        
        public string GetFileExtensions()
        {
            return ".cs";
        }


        private IEnumerable<string> GetDefaultImports()
        {
            yield return "System";
            yield return "System.Collections.Generic";
            yield return "System.Linq";
        }

        private string CodeModelVisibilityToString(CodeModelVisibility visibility)
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

        private string CodeModelTypeToString(CodeModelType type)
        {
            try
            {
                dynamic dynamicType = type;

                return MapCodeModelType(dynamicType);
            }
            catch (RuntimeBinderException e)
            {
                //TODO: string do resourcu
                throw new ModelMigrationsException(string.Format("Cannot generate property, because property type {0} is not supported", type.GetType().Name), e);
            }
        }

        protected virtual string MapCodeModelType(ScalarType type)
        {
            switch (type.Type)
            {
                case PrimitiveTypeKind.Binary:
                    return "byte[]";
                case PrimitiveTypeKind.Boolean:
                    return "bool";
                case PrimitiveTypeKind.Byte:
                    return "byte";
                case PrimitiveTypeKind.DateTime:
                    return "DateTime";
                case PrimitiveTypeKind.Time:
                    return "TimeSpan";
                case PrimitiveTypeKind.DateTimeOffset:
                    return "DateTimeOffset";
                case PrimitiveTypeKind.Decimal:
                    return "decimal";
                case PrimitiveTypeKind.Double:
                    return "double";
                case PrimitiveTypeKind.Geography:
                case PrimitiveTypeKind.GeographyPoint:
                case PrimitiveTypeKind.GeographyLineString:
                case PrimitiveTypeKind.GeographyPolygon:
                case PrimitiveTypeKind.GeographyMultiPoint:
                case PrimitiveTypeKind.GeographyMultiLineString:
                case PrimitiveTypeKind.GeographyMultiPolygon:
                case PrimitiveTypeKind.GeographyCollection:
                    return "System.Data.Entity.Spatial.DbGeography"; //TODO: mozna ne fullname ale pak musi byt using ve tride....
                case PrimitiveTypeKind.Geometry:
                case PrimitiveTypeKind.GeometryPoint:
                case PrimitiveTypeKind.GeometryLineString:
                case PrimitiveTypeKind.GeometryPolygon:
                case PrimitiveTypeKind.GeometryMultiPoint:
                case PrimitiveTypeKind.GeometryMultiLineString:
                case PrimitiveTypeKind.GeometryMultiPolygon:
                case PrimitiveTypeKind.GeometryCollection:
                    return "System.Data.Entity.Spatial.DbGeometry";
                case PrimitiveTypeKind.Guid:
                    return "Guid";
                case PrimitiveTypeKind.Single:
                    return "float";
                case PrimitiveTypeKind.SByte:
                    return "sbyte";
                case PrimitiveTypeKind.Int16:
                    return "short";
                case PrimitiveTypeKind.Int32:
                    return "int";
                case PrimitiveTypeKind.Int64:
                    return "long";
                case PrimitiveTypeKind.String:
                    return "string";
                default:
                    throw new InvalidOperationException("Invalid PrimitiveTypeKind."); //TODO: string do resourcu
            }
        }

        protected virtual string MapCodeModelType(NavigationType type)
        {
            if(type.IsCollection)
            {
                return string.Format("ICollection<{0}>", type.TargetClass);
            }
            else
            {
                return type.TargetClass;
            }
        }

        public IMappingInformationsGenerator MappingGenerator
        {
            get
            {
                return mappingGenerator;
            }
        }


        
    }
}
