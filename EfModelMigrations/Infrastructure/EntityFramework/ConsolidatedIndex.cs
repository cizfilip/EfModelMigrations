using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics;
using System.Linq;
using EfModelMigrations.Infrastructure.EntityFramework.EdmExtensions;
using System;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    internal class ConsolidatedIndex
    {
        private readonly string table;
        private IndexAttribute index;
        private readonly IDictionary<int, string> columns = new Dictionary<int, string>();

        public ConsolidatedIndex(string table, IndexAttribute index)
        {
            this.table = table;
            this.index = index;
        }

        public ConsolidatedIndex(string table, string column, IndexAttribute index)
            : this(table, index)
        {
            this.columns[index.Order] = column;
        }

        public static IEnumerable<ConsolidatedIndex> BuildIndexes(string tableName, IEnumerable<Tuple<string, EdmProperty>> columns)
        {
            var allIndexes = new List<ConsolidatedIndex>();

            foreach (var column in columns)
            {
                foreach (var index in column.Item2.IndexAnnotations()
                    .SelectMany(a => a.Indexes))
                {
                    var consolidated = index.Name == null ? null : allIndexes.FirstOrDefault(i => i.Index.Name == index.Name);
                    if (consolidated == null)
                    {
                        allIndexes.Add(new ConsolidatedIndex(tableName, column.Item1, index));
                    }
                    else
                    {
                        consolidated.Add(column.Item1, index);
                    }
                }
            }

            return allIndexes;
        }

        public IndexAttribute Index
        {
            get { return index; }
        }

        public IEnumerable<string> Columns
        {
            get { return columns.OrderBy(c => c.Key).Select(c => c.Value); }
        }

        public string Table
        {
            get { return table; }
        }

        //TODO: stringy do resourcu
        public void Add(string columnName, IndexAttribute newIndex)
        {
            Debug.Assert(index.Name == newIndex.Name);

            if (columns.ContainsKey(newIndex.Order))
            {
                throw new InvalidOperationException(
                    string.Format("The index with name '{0}' on table '{1}' has the same column order of '{2}' specified for columns '{3}' and '{4}'. Make sure a different order value is used for the IndexAttribute on each column of a multi-column index.",
                        newIndex.Name, table, newIndex.Order, columns[newIndex.Order], columnName
                    ));
                    
            }

            columns[newIndex.Order] = columnName;

            index = MergeIndexAttributes(index, newIndex, ignoreOrder: true);
        }

        public CreateIndexOperation CreateCreateIndexOperation()
        {
            var columnNames = Columns.ToArray();
            Debug.Assert(columnNames.Length > 0);
            Debug.Assert(index.Name != null || columnNames.Length == 1);

            var operation = new CreateIndexOperation
            {
                Name = index.Name ?? IndexOperation.BuildDefaultName(columnNames),
                Table = table
            };

            foreach (var columnName in columnNames)
            {
                operation.Columns.Add(columnName);
            }

            if (index.IsClusteredConfigured)
            {
                operation.IsClustered = index.IsClustered;
            }

            if (index.IsUniqueConfigured)
            {
                operation.IsUnique = index.IsUnique;
            }

            return operation;
        }

        public DropIndexOperation CreateDropIndexOperation()
        {
            return (DropIndexOperation)CreateCreateIndexOperation().Inverse;
        }

        private IndexAttribute MergeIndexAttributes(IndexAttribute me, IndexAttribute other, bool ignoreOrder = false)
        {
            if (ReferenceEquals(me, other)
                || other == null)
            {
                return me;
            }

            var merged = me.Name != null
                ? new IndexAttribute(me.Name)
                : other.Name != null ? new IndexAttribute(other.Name) : new IndexAttribute();

            if (!ignoreOrder)
            {
                if (me.Order != -1)
                {
                    merged.Order = me.Order;
                }
                else if (other.Order != -1)
                {
                    merged.Order = other.Order;
                }
            }

            if (me.IsClusteredConfigured)
            {
                merged.IsClustered = me.IsClustered;
            }
            else if (other.IsClusteredConfigured)
            {
                merged.IsClustered = other.IsClustered;
            }

            if (me.IsUniqueConfigured)
            {
                merged.IsUnique = me.IsUnique;
            }
            else if (other.IsUniqueConfigured)
            {
                merged.IsUnique = other.IsUnique;
            }

            return merged;
        }
    }
}
