using EnvDTE;
using System;
using System.Collections.Generic;
using EfModelMigrations.Runtime.Extensions;
using System.IO;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Resources;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class HistoryTracker : MarshalByRefObject
    {
        private Project modelProject;
        private IDictionary<string, HistoryItem> history;

        public HistoryTracker(Project modelProject)
        {
            this.history = new Dictionary<string, HistoryItem>();
            this.modelProject = modelProject;
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

        //TODO: ted se nejspis cely ProjectItem serializuje protoze jsme marshalbyref
        public void MarkItemDeleted(ProjectItem item)
        {
            MarkItemModifiedOrDeleted(item);
        }

        public void MarkItemModified(ProjectItem item)
        {
            MarkItemModifiedOrDeleted(item);
        }

        public void Reset()
        {
            history.Clear();
        }

        public void Restore()
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
                throw new ModelMigrationsException(Strings.HistoryTracker_Error, e);
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
                    RestoreContentOfProjectItem(projectItem, historyItem.Content);
                    break;
                default:
                    throw new InvalidOperationException(Strings.HistoryTracker_InvalidItemType);
            }
        }

        private void RestoreContentOfProjectItem(ProjectItem projectItem, string content)
        {
            if (!projectItem.get_IsOpen())
            {
                File.WriteAllText(GetItemFullPath(projectItem), content);
            }
            else
            {
                var document = (TextDocument)projectItem.Document.Object("TextDocument");
                var endPoint = document.EndPoint;
                document.StartPoint.CreateEditPoint().ReplaceText(endPoint, content, EnvDteExtensions.AllvsEPReplaceTextOptionsFlags());
                projectItem.Save();
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
            string key = GetItemFullPath(item);

            if (!history.ContainsKey(key))
            {
                string oldContent = GetItemContent(key);
                history.Add(key, HistoryItem.ModifiedOrDeletedItem(oldContent));
            }
        }

        private string GetItemFullPath(ProjectItem item)
        {
            if (item.FileCount != 1)
                throw new InvalidOperationException(Strings.HistoryTracker_MoreThanOneFileInProjectItem(item.Name));

            return item.FileNames[1];
        }

        private string GetItemContent(string fullPath)
        {
            try
            {
                return File.ReadAllText(fullPath);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(Strings.HistoryTracker_CannotGetOldContent(fullPath), e);
            }
        }
        
        #endregion

        #region Helper classes
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
