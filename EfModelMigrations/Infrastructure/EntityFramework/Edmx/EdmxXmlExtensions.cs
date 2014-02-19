using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EfModelMigrations.Exceptions;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Mapping;

namespace EfModelMigrations.Infrastructure.EntityFramework.Edmx
{
    //TODO: predelat tohle API aby se zbytecne nektere veci nemuseli volat vicekrat behem buildovani operaci
    //napr misto GetColumnModelsForClass udelat metodu GetColumnModelsForTable a bude se predavat rovnou jmeno tabulky
    internal static class EdmxXmlExtensions
    {
        private static readonly PrimitiveTypeKind[] validIdentityTypes =
            {
                PrimitiveTypeKind.Byte,
                PrimitiveTypeKind.Decimal,
                PrimitiveTypeKind.Guid,
                PrimitiveTypeKind.Int16,
                PrimitiveTypeKind.Int32,
                PrimitiveTypeKind.Int64
            };


        //TODO: Handlovat vyjimky
        public static string GetTableName(this XDocument edmx, string classFullName)
        {
            var entitySet = GetStorageEntitySetFromClassName(edmx, classFullName);

            return GetSchemaQualifiedTableName(entitySet.SchemaAttribute(), entitySet.TableAttribute());
        }

        public static string GetTableNameWithoutSchema(this XDocument edmx, string classFullName)
        {
            var entitySet = GetStorageEntitySetFromClassName(edmx, classFullName);

            return entitySet.TableAttribute();
        }

        //TODO: EF predpoklada ze ve Storage modeluje EntitySet name stejne jako EntityType name, ja to nepredpokladam a mozna zbytecne hledam v edmx porad dokola....
        public static IEnumerable<ColumnModel> GetColumnModelsForClass(this XDocument edmx, string classFullName)
        {
            var storageEntityType = GetStorageEntityTypeFromClassName(edmx, classFullName);

            var storageEntitySetName = GetStorageEntitySetFromClassName(edmx, classFullName).NameAttribute();

            foreach (var prop in storageEntityType.Descendants(EdmxNames.Ssdl.PropertyNames))
            {
                yield return BuildColumnModel(edmx, prop, storageEntitySetName);
            }
        }

        public static string GetColumnStorageType(this XDocument edmx, string tableFullName, string columnName)
        {
            return GetStorageEntityTypeFromTableName(edmx, tableFullName).FindColumnInStorageEntityType(columnName).TypeAttribute();
        }

        public static IEnumerable<string> GetTableKeyColumnNamesForClass(this XDocument edmx, string classFullName)
        {
            var storageEntityType = GetStorageEntityTypeFromClassName(edmx, classFullName);

            foreach (var key in storageEntityType.Descendants(EdmxNames.Ssdl.PropertyRefNames))
            {
                yield return key.NameAttribute();
            }
        }

        public static string GetColumnName(this XDocument edmx, string classFullName, string propertyName)
        {
            var mappingFragment = GetEntityTypeMappingFragment(edmx, classFullName);
            return mappingFragment.Descendants(EdmxNames.Msl.ScalarPropertyNames)
                .Where(m => m.NameAttribute().EqualsOrdinalIgnoreCase(propertyName))
                .SingleOrThrow(
                    () => new DbMigrationBuilderException(string.Format("Cannot find scalar property with name {0} in entity type mapping for class {1}.", propertyName, classFullName)),
                    () => new DbMigrationBuilderException(string.Format("More than one scalar property with name {0} was found in entity type mapping for class {1}.", propertyName, classFullName)))
                .ColumnNameAttribute();
        }

        

        public static IEnumerable<string> GetComplexTypeProperties(this XDocument edmx, string complexTypeName)
        {
            return edmx.Descendants(EdmxNames.Csdl.ComplexTypeNames).Where(e => e.NameAttribute().EqualsOrdinalIgnoreCase(complexTypeName))
                .SingleOrThrow(
                    () => new DbMigrationBuilderException(string.Format("Cannot find complex type {0}.", complexTypeName)),
                    () => new DbMigrationBuilderException(string.Format("More than one complex type {0} found.", complexTypeName)))
                .Descendants(EdmxNames.Csdl.PropertyNames)
                .Select(p => p.NameAttribute());
        }

        public static bool IsColumnIdentity(this XDocument edmx, string columnName, string tableFullName)
        {
            var storeGeneratedPatternAttribute = GetStorageEntityTypeFromTableName(edmx, tableFullName)
                .FindColumnInStorageEntityType(columnName)
                .StoreGeneratedPatternAttribute();

            if (!string.IsNullOrEmpty(storeGeneratedPatternAttribute) && storeGeneratedPatternAttribute == "Identity")
            {
                return true;
            }

            return false;
        }

        //TODO stringy do resourcu
        public static IEnumerable<Tuple<string, string>> GetDependentColumns(this XDocument edmx, string tableFullName, string columnName)
        {
            var storageEntityType = GetStorageEntityTypeFromTableName(edmx, tableFullName);
            var storageEntityTypeName = storageEntityType.NameAttribute();

            var associations = edmx.Descendants(EdmxNames.Ssdl.AssociationNames)
                .Where(a => a.Descendants(EdmxNames.Ssdl.EndNames).Select(e => RemoveAliasFromName(e.TypeAttribute())).Contains(storageEntityTypeName));

            foreach (var assoc in associations)
            {
                string roleName = assoc.Descendants(EdmxNames.Ssdl.EndNames).Single(e => RemoveAliasFromName(e.TypeAttribute()) == storageEntityTypeName).RoleAttribute();
                
                //TODO: Association element nemusí mít dle specifikace vždy potomka ReferentialConstraint - když není musíme se dívat do AssociationSetMapping (pozn. dá se předpokládat, že bude mít vždy jelikož tohle xml generuje ef - podívat do zdrojáků jak...)
                var refConstraint = assoc.Descendants(EdmxNames.Ssdl.ReferentialConstraintNames).Single();
                
                if (refConstraint.Descendants(EdmxNames.Ssdl.PrincipalNames).Single().RoleAttribute() == roleName)
                {
                    var dependent = assoc.Descendants(EdmxNames.Ssdl.DependentNames).Single();

                    var dependentStorageTypeName = RemoveAliasFromName(
                                                assoc.Descendants(EdmxNames.Ssdl.EndNames).Single(e => e.RoleAttribute() == dependent.RoleAttribute()).TypeAttribute()
                                             );

                    var dependentStorageEntitySet = edmx.Descendants(EdmxNames.Ssdl.EntitySetNames).Where(e => e.EntityTypeAttribute().EqualsOrdinalIgnoreCase(dependentStorageTypeName))
                        .SingleOrThrow(
                            () => new DbMigrationBuilderException(string.Format("Cannot find Storage Entity Set information for storage entity type {0}.", dependentStorageTypeName)),
                            () => new DbMigrationBuilderException(string.Format("More than one Storage Entity Set found for storage entity type {0}.", dependentStorageTypeName))
                        );

                    var dependentTableName = GetSchemaQualifiedTableName(dependentStorageEntitySet.SchemaAttribute(), dependentStorageEntitySet.TableAttribute());

                    yield return Tuple.Create(dependentTableName, dependent.Descendants(EdmxNames.Ssdl.PropertyRefNames).Single().NameAttribute());
                }
            }
        }


        #region Private Helper methods

        //TODO: stringy do resourcu
        private static XElement FindColumnInStorageEntityType(this XElement storageEntityType, string columnName)
        {
            return storageEntityType.Descendants(EdmxNames.Ssdl.PropertyNames)
                .Where(p => p.NameAttribute().EqualsOrdinalIgnoreCase(columnName))
                .SingleOrThrow(
                    () => new DbMigrationBuilderException(string.Format("Cannot find column {0} in table {1}.", columnName, storageEntityType.NameAttribute())),
                    () => new DbMigrationBuilderException(string.Format("More than one column {0} in table {1} found.", columnName, storageEntityType.NameAttribute())));
        }


        //TODO: stringy do resourcu
        private static XElement GetStorageEntityTypeFromTableName(XDocument edmx, string tableFullName)
        {
            var parsedTableName = ParseTableFullName(tableFullName);
            return edmx.Descendants(EdmxNames.Ssdl.EntitySetNames)
                .Where(e => e.SchemaAttribute().EqualsOrdinalIgnoreCase(parsedTableName.Item1) && e.TableAttribute().EqualsOrdinalIgnoreCase(parsedTableName.Item2))
                .SingleOrThrow(
                    () => new DbMigrationBuilderException(string.Format("Cannot find storage entity type for table with name {0}.", tableFullName)),
                    () => new DbMigrationBuilderException(string.Format("More than one storage entity typs found for table with name {0}.", tableFullName)));
        }

        //TODO: message do resourcu
        private static XElement GetEntityTypeMappingFragment(XDocument edmx, string classFullName)
        {
            return edmx.Descendants(EdmxNames.Msl.EntityTypeMappingNames)
                .Where(e => e.TypeNameAttribute().EqualsOrdinal(classFullName))
                .SingleOrThrow(
                    () => new DbMigrationBuilderException(string.Format("Cannot find entity type mapping information for class {0}.", classFullName)),
                    () => new DbMigrationBuilderException(string.Format("More than one entity type mapping find for class {0}.", classFullName)))
                .Descendants(EdmxNames.Msl.MappingFragmentNames)
                .SingleOrThrow(
                    () => new DbMigrationBuilderException(string.Format("No mapping fragments found in entity type mapping for class {0}.", classFullName)),
                    () => new DbMigrationBuilderException(string.Format("Class {0} is mapped to more than one table.", classFullName)));
        }

        //TODO: message do resourcu
        private static XElement GetStorageEntitySetFromClassName(XDocument edmx, string classFullName)
        {
            //TODO: handlovat mozna IsTypeOf() v TypeName attributu
            string storeSet = GetEntityTypeMappingFragment(edmx, classFullName)
                .StoreEntitySetAttribute();

            var storageEntitySet = edmx.Descendants(EdmxNames.Ssdl.EntitySetNames)
                .Where(e => e.NameAttribute().EqualsOrdinal(storeSet))
                .SingleOrThrow(
                    () => new DbMigrationBuilderException(string.Format("Cannot find Storage Entity Set information for class {0}.", classFullName)),
                    () => new DbMigrationBuilderException(string.Format("More than one Storage Entity Set found for class {0}.", classFullName)));

            return storageEntitySet;
        }


        //TODO: message do resourcu
        private static XElement GetStorageEntityTypeFromClassName(XDocument edmx, string classFullName)
        {
            string storageEntityTypeName = RemoveAliasFromName(GetStorageEntitySetFromClassName(edmx, classFullName).EntityTypeAttribute());

            var storageEntityType = edmx.Descendants(EdmxNames.Ssdl.EntityTypeNames)
                .Where(e => e.NameAttribute().EqualsOrdinal(storageEntityTypeName))
                .SingleOrThrow(
                    () => new DbMigrationBuilderException(string.Format("Cannot find Storage Entity Type information for class {0}.", classFullName)),
                    () => new DbMigrationBuilderException(string.Format("More than one Storage Entity Type found for class {0}.", classFullName)));

            return storageEntityType;
        }

        private static string GetSchemaQualifiedTableName(string schema, string table)
        {
            return schema + "." + table;
        }

        private static Tuple<string, string> ParseTableFullName(string tablefullName)
        {
            var splitted = tablefullName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return Tuple.Create(splitted.First(), splitted.Last());
        }

        private static string RemoveAliasFromName(string name)
        {
            return name.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        #endregion



        #region BuilColumnModel Methods

        private static ColumnModel BuildColumnModel(XDocument edmx,
            XElement property, string entitySetName)
        {

            var nameAttribute = property.NameAttribute();
            var nullableAttribute = property.NullableAttribute();
            var maxLengthAttribute = property.MaxLengthAttribute();
            var precisionAttribute = property.PrecisionAttribute();
            var scaleAttribute = property.ScaleAttribute();
            var storeGeneratedPatternAttribute = property.StoreGeneratedPatternAttribute();
            var storeType = property.TypeAttribute();


            DbProviderInfo providerInfo = edmx.GetDbProviderInfo();
            StoreItemCollection storeItemCollection = edmx.GetStoreItemCollection();


            var entityType
                = storeItemCollection
                    .OfType<EntityType>()
                    .Single(et => et.Name.EqualsOrdinalIgnoreCase(entitySetName));

            var edmProperty
                = entityType.Properties[nameAttribute];

            var providerManifest = GetProviderManifest(providerInfo);

            var typeUsage = providerManifest.GetEdmType(edmProperty.TypeUsage);

            var defaultStoreTypeName = providerManifest.GetStoreType(typeUsage).EdmType.Name;

            var column
                = new ColumnModel(((PrimitiveType)edmProperty.TypeUsage.EdmType).PrimitiveTypeKind, typeUsage)
                {
                    Name = nameAttribute,
                    IsNullable
                        = !string.IsNullOrWhiteSpace(nullableAttribute)
                          && !Convert.ToBoolean(nullableAttribute, CultureInfo.InvariantCulture)
                              ? false
                              : (bool?)null,
                    MaxLength
                        // Setting "Max" is equivalent to not setting anything
                        = !string.IsNullOrWhiteSpace(maxLengthAttribute) && !maxLengthAttribute.EqualsOrdinalIgnoreCase("Max")
                              ? Convert.ToInt32(maxLengthAttribute, CultureInfo.InvariantCulture)
                              : (int?)null,
                    Precision
                        = !string.IsNullOrWhiteSpace(precisionAttribute)
                              ? Convert.ToByte(precisionAttribute, CultureInfo.InvariantCulture)
                              : (byte?)null,
                    Scale
                        = !string.IsNullOrWhiteSpace(scaleAttribute)
                              ? Convert.ToByte(scaleAttribute, CultureInfo.InvariantCulture)
                              : (byte?)null,
                    StoreType
                        = !storeType.EqualsOrdinalIgnoreCase(defaultStoreTypeName)
                              ? storeType
                              : null
                };

            column.IsIdentity
                = !string.IsNullOrWhiteSpace(storeGeneratedPatternAttribute)
                  && storeGeneratedPatternAttribute.EqualsOrdinalIgnoreCase("Identity")
                  && validIdentityTypes.Contains(column.Type);

            Facet facet;
            if (typeUsage.Facets.TryGetValue(DbProviderManifest.FixedLengthFacetName, true, out facet)
                && facet.Value != null
                && (bool)facet.Value)
            {
                column.IsFixedLength = true;
            }

            if (typeUsage.Facets.TryGetValue(DbProviderManifest.UnicodeFacetName, true, out facet)
                && facet.Value != null
                && !(bool)facet.Value)
            {
                column.IsUnicode = false;
            }

            var isComputed
                = !string.IsNullOrWhiteSpace(storeGeneratedPatternAttribute)
                  && storeGeneratedPatternAttribute.EqualsOrdinalIgnoreCase("Computed");

            if ((column.Type == PrimitiveTypeKind.Binary)
                && (typeUsage.Facets.TryGetValue(DbProviderManifest.MaxLengthFacetName, true, out facet)
                    && (facet.Value is int)
                    && ((int)facet.Value == 8))
                && isComputed)
            {
                column.IsTimestamp = true;
            }

            return column;
        }


        private static DbProviderManifest GetProviderManifest(DbProviderInfo providerInfo)
        {
            var providerFactory = DbConfiguration.DependencyResolver.GetService(typeof(DbProviderFactory), providerInfo.ProviderInvariantName) as DbProviderFactory;

            var providerServices = GetProviderServices(providerFactory);

            return providerServices.GetProviderManifest(providerInfo.ProviderManifestToken);
        }

        private static DbProviderServices GetProviderServices(DbProviderFactory factory)
        {
            // The EntityClient provider invariant name is not normally registered so we can't use
            // the normal method for looking up this factory.
            if (factory is EntityProviderFactory)
            {
                //TODO: Hack - EF zde pouziva return EntityProviderServices.Instance; ale to jest internal, 
                //ovsem podle zdrojaku udela nasledujici radka to same
                return (factory as IServiceProvider).GetService(typeof(DbProviderServices)) as DbProviderServices;
            }

            var invariantName = DbConfiguration.DependencyResolver.GetService(typeof(IProviderInvariantName), factory) as IProviderInvariantName;

            return DbConfiguration.DependencyResolver.GetService(typeof(DbProviderServices), invariantName.Name) as DbProviderServices;
        }

        private static StoreItemCollection GetStoreItemCollection(this XDocument edmx)
        {
            var storeItemCollection
                = new StoreItemCollection(
                    new[]
                        {
                            edmx.GetSsdlSchemaElement().CreateReader()
                        });

            return storeItemCollection;
        }

        private static DbProviderInfo GetDbProviderInfo(this XDocument edmx)
        {
            var ssdlSchemaElement = edmx.GetSsdlSchemaElement();

            return new DbProviderInfo(
                ssdlSchemaElement.ProviderAttribute(),
                ssdlSchemaElement.ProviderManifestTokenAttribute());
        }

        private static XElement GetSsdlSchemaElement(this XDocument edmx)
        {
            return edmx.Descendants(EdmxNames.Ssdl.SchemaNames).Single();
        }

        #endregion
    }
}
