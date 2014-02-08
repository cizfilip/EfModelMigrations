using EnvDTE;
using System;
using System.Collections.Generic;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    [Serializable]
    internal class HistoryTracker
    {
        private IDictionary<string, HistoryItem> history;

        public HistoryTracker()
        {
            this.history = new Dictionary<string, HistoryItem>();
        }

        public void MarkItemAdded(ProjectItem item)
        {
            if (item == null)
            {
                //TODO: mozna vyhodit vyjimku ne spokojene nic nepridat do historie....
                return;
            }

            string key = GetItemKey(item);
            if (!history.ContainsKey(key))
            {
                history.Add(key, HistoryItem.AddedItem());
            }
        }

        public void MarkItemDeleted(ProjectItem item)
        {
            MarkItemModifiedOrDeleted(item);
        }

        public void MarkItemModified(ProjectItem item)
        {
            MarkItemModifiedOrDeleted(item);
        }


        private void MarkItemModifiedOrDeleted(ProjectItem item)
        {
            if (item == null)
            {
                //TODO: mozna vyhodit vyjimku ne spokojene nic nepridat do historie....
                return;
            }

            string key = GetItemKey(item);
            if (!history.ContainsKey(key))
            {
                history.Add(key, HistoryItem.ModifiedOrDeletedItem(GetItemContent(item)));
            }
        }

        private string GetItemKey(ProjectItem item)
        {
            var document = item.Document;
            if (document != null)
            {
                return document.FullName;
            }

            //TODO: asi spis vyhazovat vyjimku
            return null;
        }

        private string GetItemContent(ProjectItem item)
        {
            var textDocument = item.Document as TextDocument;
            if (textDocument != null)
            {
                var editPoint = textDocument.StartPoint.CreateEditPoint();
                var content = editPoint.GetText(textDocument.EndPoint);

                return content;
            }

            return null;
        }


        public void Restore()
        {

        }

        [Serializable]
        private class HistoryItem
        {
            public HistoryItemType Type { get; set; }
            public string Content { get; set; }

            public static HistoryItem AddedItem()
            {
                return new HistoryItem()
                {
                    Type = HistoryItemType.Added
                };
            }

            public static HistoryItem ModifiedOrDeletedItem(string content)
            {
                return new HistoryItem()
                {
                    Type = HistoryItemType.ModifiedOrDeleted,
                    Content = content
                };
            }
        }

        private enum HistoryItemType
        {
            Added,
            ModifiedOrDeleted
        }
    }

    

}
