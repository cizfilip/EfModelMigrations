using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public class ClassCodeModel
    {
        internal ClassCodeModel(
            string name,
            TableName tableName,
            CodeModelVisibility visibility,
            string baseType,
            IEnumerable<string> implementedInterfaces,
            IEnumerable<PrimitivePropertyCodeModel> properties,
            IEnumerable<NavigationPropertyCodeModel> navigationProperties,
            IEnumerable<PrimitivePropertyCodeModel> primaryKeys)
        {
            Check.NotEmpty(name, "name");
            Check.NotNull(tableName, "tableName");

            this.Name = name;
            this.TableName = tableName;
            this.Visibility = visibility;
            this.BaseType = baseType;
            this.ImplementedInterfaces = implementedInterfaces ?? Enumerable.Empty<string>();
            this.Properties = properties ?? Enumerable.Empty<PrimitivePropertyCodeModel>();
            this.NavigationProperties = navigationProperties ?? Enumerable.Empty<NavigationPropertyCodeModel>();
            this.PrimaryKeys = primaryKeys ?? Enumerable.Empty<PrimitivePropertyCodeModel>();
        }

        public string Name { get; private set; }
        public TableName TableName { get; private set; }
        public CodeModelVisibility Visibility { get; private set; }
        public string BaseType { get; private set; }
        public IEnumerable<string> ImplementedInterfaces { get; private set; }
        public IEnumerable<PrimitivePropertyCodeModel> Properties { get; private set; }
        public IEnumerable<NavigationPropertyCodeModel> NavigationProperties { get; private set; }
        public IEnumerable<PrimitivePropertyCodeModel> PrimaryKeys { get; private set; }

        public EntityType StoreEntityType { get; internal set; }
        public EntityType ConceptualEntityType { get; internal set; }

        public ClassModel ToClassModel()
        {
            return new ClassModel(Name, TableName, Visibility);
        }
    }

    public sealed class TableName
    {
        public string Table { get; private set; }
        public string Schema { get; private set; }

        public TableName(string table, string schema)
        {
            Check.NotEmpty(table, "table");

            this.Table = table;
            this.Schema = schema;
        }
    }

}
