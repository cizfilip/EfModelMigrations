using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class HistoryTrackerWrapper
    {
        private HistoryTracker historyTracker;

        public HistoryTrackerWrapper(HistoryTracker historyTracker)
        {
            this.historyTracker = historyTracker;
        }

        public void MarkItemAdded(string fullPath)
        {
            historyTracker.MarkItemAdded(fullPath);
        }

        public void MarkItemDeleted(ProjectItem item)
        {
            historyTracker.MarkItemDeleted(
                GetItemFullPath(item),
                GetItemContent(item)
                );
        }

        public void MarkItemModified(ProjectItem item)
        {
            historyTracker.MarkItemModified(
                GetItemFullPath(item),
                GetItemContent(item)
                );
        }

        private string GetItemFullPath(ProjectItem item)
        {
            var document = GetProjectItemDocument(item);
            if (document != null)
            {
                return document.FullName;
            }

            //TODO: asi spis vyhazovat vyjimku
            return null;
        }

        private string GetItemContent(ProjectItem item)
        {
            var textDocument = GetProjectItemDocument(item) as TextDocument;
            if (textDocument != null)
            {
                var editPoint = textDocument.StartPoint.CreateEditPoint();
                var content = editPoint.GetText(textDocument.EndPoint);

                return content;
            }

            return null;
        }

        private Document GetProjectItemDocument(ProjectItem item)
        {
            if (!item.get_IsOpen())
            {
                item.Open();
            }
            return item.Document;
        }
    }
}
