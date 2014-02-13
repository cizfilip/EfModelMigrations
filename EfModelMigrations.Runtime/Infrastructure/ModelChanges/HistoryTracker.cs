using EnvDTE;
using System;
using System.Collections.Generic;
using EfModelMigrations.Runtime.Extensions;
using System.IO;
using EfModelMigrations.Exceptions;

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

        public void MarkItemAdded(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                //TODO: mozna vyhodit vyjimku ne spokojene nic nepridat do historie....
                return;
            }
            string key = fullPath;
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


        public void Restore(Project modelProject)
        {
            try
            {
                foreach (var item in history)
                {
                    ProjectItem projectItem;
                    if (TryFindProjectItem(modelProject, item.Key, out projectItem))
                    {
                        RestoreFoundHistoryItem(modelProject, projectItem, item.Value);
                    }
                    else
                    {
                        RestoreLostHistoryItem(modelProject, item.Key, item.Value);
                    }
                }
            }
            catch (Exception e)
            {
                //TODO: wrap e in new FatalError exception?
                throw new ModelMigrationsException("Error during reverting history! See inner exception", e); //TODO: string do resourcu
            }
        }

        private void RestoreLostHistoryItem(Project modelProject, string path, HistoryItem historyItem)
        {
            if (historyItem.Type == HistoryItemType.ModifiedOrDeleted)
            {
                modelProject.AddContentToProjectFromAbsolutePath(path, historyItem.Content);
            }
        }

        private void RestoreFoundHistoryItem(Project modelProject, ProjectItem projectItem, HistoryItem historyItem)
        {
            switch (historyItem.Type)
            {
                case HistoryItemType.Added:
                    projectItem.Delete();
                    break;
                case HistoryItemType.ModifiedOrDeleted:
                    var document = GetProjectItemDocument(projectItem) as TextDocument;
                    var endPoint = document.EndPoint;
                    document.StartPoint.CreateEditPoint().ReplaceText(endPoint, historyItem.Content, (int)vsEPReplaceTextOptions.vsEPReplaceTextKeepMarkers);
                    break;
                default:
                    throw new InvalidOperationException("Invalid history item type."); //TODO: string do resaurcu
            }
        }

        private bool TryFindProjectItem(Project modelProject, string fullPath, out ProjectItem projectItem)
        {
            try
            {
                string[] relativePath = GetRelativePath(modelProject, fullPath)
                    .Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                            StringSplitOptions.RemoveEmptyEntries);


                ProjectItems projectItems = modelProject.ProjectItems;

                for (int i = 0; i < relativePath.Length - 1; i++)
                {
                    projectItems = projectItems.Item(relativePath[i]).ProjectItems;
                }

                ProjectItem itemToReturn = projectItems.Item(relativePath[relativePath.Length - 1]);
                if (itemToReturn != null)
                {
                    projectItem = itemToReturn;
                    return true;
                }
            }
            catch (ArgumentException) //Cannot find (projectItems.Item() throws) 
            {
            }
            projectItem = null;
            return false;
        }

        private string GetRelativePath(Project modelProject, string fullPath)
        {
            string rootPath = modelProject.GetProjectDir();

            return fullPath.Replace(rootPath, "");
        }

        #region Private methods

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
                //TODO: nelze volat jelikoz to vraci object Window a ten neni serializovatelny - tudiz to spadne behem prenaseni mezi appdomenama
                item.Open();
            }
            return item.Document;
        }

        #endregion

        #region Helper classes
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

        #endregion
    }



}
