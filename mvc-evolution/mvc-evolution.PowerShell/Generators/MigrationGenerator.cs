using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using mvc_evolution.PowerShell.Extensions;
using System.IO;
using System.Xml;
using System.IO.Compression;

namespace mvc_evolution.PowerShell.Generators
{
    class MigrationGenerator
    {
        private Assembly efAssembly;
        private DbContext dbContext;
        private DbMigrationsConfiguration migrationConfiguration;


        public MigrationGenerator(Assembly efAssembly, DbContext dbContext, DbMigrationsConfiguration migrationConfiguration)
        {
            this.efAssembly = efAssembly;
            this.dbContext = dbContext;
            this.migrationConfiguration = migrationConfiguration;
        }

        public ScaffoldedMigration GenerateMigration(string migrationName, IEnumerable<MigrationOperation> operations)
        {
            var migrationNameAndId = new MigrationNameCreator(efAssembly, migrationConfiguration).CreateMigrationNameAndIdFromName(migrationName);

            migrationName = migrationNameAndId.Item1;
            string migrationId = migrationNameAndId.Item2;

            var contextModel = GetContextModel();

            var compressedModel = CompressModel(contextModel);


            var scaffoldedMigration = migrationConfiguration.CodeGenerator.Generate(migrationId, operations, null, compressedModel, migrationConfiguration.MigrationsNamespace, migrationName);

            scaffoldedMigration.MigrationId = migrationId;
            scaffoldedMigration.Directory = migrationConfiguration.MigrationsDirectory;
            scaffoldedMigration.IsRescaffold = false;

            //TODO: get default schema from EF internals.
            //scaffoldedMigration.Resources.Add(DefaultSchemaResourceKey, _defaultSchema);
            scaffoldedMigration.Resources.Add("DefaultSchema", "dbo");

            return scaffoldedMigration;
        }


        private XDocument GetContextModel()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(
                    memoryStream, new XmlWriterSettings
                                      {
                                          Indent = true
                                      }))
                {
                    EdmxWriter.WriteEdmx(dbContext, xmlWriter);
                }

                memoryStream.Position = 0;

                return XDocument.Load(memoryStream);
            }
        }

        private string CompressModel(XDocument model)
        {
            using (var outStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outStream, CompressionMode.Compress))
                {
                    model.Save(gzipStream);
                }

                return Convert.ToBase64String(outStream.ToArray());
            }
        }
    }

    class MigrationOperationBuilder
    {
        private MetadataWorkspace metadata;

        public MigrationOperationBuilder(DbContext context)
        {
            this.metadata = (context as IObjectContextAdapter).ObjectContext.MetadataWorkspace;
        }

        public CreateTableOperation BuildCreateTableOperation(Type forType)
        {
            string tableName = metadata.GetTableNameForType(forType);

            CreateTableOperation createTableOperation = new CreateTableOperation(tableName);

            IEnumerable<ColumnModel> columns = metadata.GetTableColumnsForType(forType).Select(prop => CreateColumnModel(prop));

            foreach (var column in columns)
            {
                createTableOperation.Columns.Add(column);
            }


            var addPrimaryKeyOperation = new AddPrimaryKeyOperation();

            var keyMembers = metadata.GetTableKeyColumnsForType(forType);

            foreach (var key in keyMembers)
            {
                addPrimaryKeyOperation.Columns.Add(key.Name);
            }

            createTableOperation.PrimaryKey = addPrimaryKeyOperation;

            return createTableOperation;
        }

        private ColumnModel CreateColumnModel(EdmProperty property)
        {
            string columnName = property.Name;
            bool columnNullable = property.Nullable;
            int? columnMaxLength = property.MaxLength;
            byte? columnPrecision = property.Precision;
            byte? columnScale = property.Scale;
            StoreGeneratedPattern columnStoreGeneratedPattern = property.StoreGeneratedPattern;
            var columnStoreType = property.TypeName;

            //var entityType
            //    = modelMetadata.StoreItemCollection
            //                   .OfType<EntityType>()
            //                   .Single(et => et.Name.EqualsIgnoreCase(entitySetName));

            var typeUsage = property.TypeUsage;
            //var typeUsage = modelMetadata.ProviderManifest.GetEdmType(property.TypeUsage);

            //var defaultStoreTypeName = modelMetadata.ProviderManifest.GetStoreType(typeUsage).EdmType.Name;


            var column
                = new ColumnModel(property.PrimitiveType.PrimitiveTypeKind, typeUsage)
                //= new ColumnModel(((PrimitiveType)property.TypeUsage.EdmType).PrimitiveTypeKind, typeUsage)
                {
                    Name = columnName,
                    IsNullable = columnNullable,
                    MaxLength = columnMaxLength,
                    Precision = columnPrecision,
                    Scale = columnScale,
                    StoreType = columnStoreType
                    //StoreType
                    //    = !string.Equals(columnStoreType, defaultStoreTypeName, StringComparison.OrdinalIgnoreCase)
                    //          ? columnStoreType
                    //          : null
                };

            column.IsIdentity = columnStoreGeneratedPattern.HasFlag(StoreGeneratedPattern.Identity);
            //TODO: check if identity type is valid type (EdmModelDiff does this)
            //&& _validIdentityTypes.Contains(column.Type);

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

            var isComputed = columnStoreGeneratedPattern.HasFlag(StoreGeneratedPattern.Computed);

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

    }

    class MigrationNameCreator
    {
        private DbMigrationsConfiguration configuration;
        private Assembly efAssembly;
        private Type migrationAssemblyType;


        public MigrationNameCreator(Assembly efAssembly, DbMigrationsConfiguration configuration)
        {
            this.configuration = configuration;
            this.efAssembly = efAssembly;

            this.migrationAssemblyType = efAssembly.GetType("System.Data.Entity.Migrations.Infrastructure.MigrationAssembly", true);
        }

        public Tuple<string, string> CreateMigrationNameAndIdFromName(string migrationName)
        {
            string retName = UniquifyName(migrationName);
            string retId = GetMigrationId(retName);

            return new Tuple<string, string>(retName, retId);
        }

        private string UniquifyName(string migrationName)
        {
            var uniquifyMethod = migrationAssemblyType.GetMethod("UniquifyName");

            var migrationAssembly = Activator.CreateInstance(migrationAssemblyType,
                new object[] { configuration.MigrationsAssembly, configuration.MigrationsNamespace });

            string uniqMigrationName = uniquifyMethod.Invoke(migrationAssembly, new object[] { migrationName }) as string;

            return uniqMigrationName;
        }

        private string GetMigrationId(string migrationName)
        {
            var createMigrationIdMethod = migrationAssemblyType.GetMethod("CreateMigrationId");

            string migrationId = createMigrationIdMethod.Invoke(null, new object[] { migrationName }) as string;

            return migrationId;
        }
    }
}


