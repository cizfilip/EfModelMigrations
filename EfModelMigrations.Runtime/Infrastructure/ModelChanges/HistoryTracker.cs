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

        public void MarkItemDeleted(string fullPath, string oldContent)
        {
            MarkItemModifiedOrDeleted(fullPath, oldContent);
        }

        public void MarkItemModified(string fullPath, string oldContent)
        {
            MarkItemModifiedOrDeleted(fullPath, oldContent);
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
                    document.StartPoint.CreateEditPoint().ReplaceText(endPoint, historyItem.Content, (int)(vsEPReplaceTextOptions.vsEPReplaceTextAutoformat | vsEPReplaceTextOptions.vsEPReplaceTextNormalizeNewlines | vsEPReplaceTextOptions.vsEPReplaceTextTabsSpaces | vsEPReplaceTextOptions.vsEPReplaceTextKeepMarkers));
                    //TODO: po pridani mozna pouzivat metodu .SmartFormat na editPointu, která snad dela to co ctrl+k,d
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

        private void MarkItemModifiedOrDeleted(string fullPath, string oldContent)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                //TODO: mozna vyhodit vyjimku ne spokojene nic nepridat do historie....
                return;
            }
            if (string.IsNullOrEmpty(oldContent))
            {
                //TODO: mozna vyhodit vyjimku ne spokojene nic nepridat do historie....
                return;
            }

            if (!history.ContainsKey(fullPath))
            {
                history.Add(fullPath, HistoryItem.ModifiedOrDeletedItem(oldContent));
            }
        }

        private Document GetProjectItemDocument(ProjectItem item)
        {
            if (!item.get_IsOpen())
            {
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
