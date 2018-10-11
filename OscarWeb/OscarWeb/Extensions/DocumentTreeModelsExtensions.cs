using System.Collections.Generic;
using System.Linq;

using Common.Models;
using Kendo.Mvc.UI;

namespace OscarWeb.Extensions
{
    
    public static class DocumentTreeModelsExtensions
    {
        /// <summary>
        /// Convert a list of documents into a Kendo UI tree view for display on the DocumentManager.cshtml page.
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        public static List<TreeViewItemModel> ToKendoTreeViewItemModelList(this DocumentTreeModels documents)
        {
            List<Kendo.Mvc.UI.TreeViewItemModel> result = null;
            if (documents?.Documents == null || !documents.Documents.Any()) return null;

            var topMenuItems = DocumentTreeModelsExtensions.GetTopLevelItems(documents, 0);

            result = new List<TreeViewItemModel>();

            foreach (var topMenuItem in topMenuItems)
            {
                TreeViewItemModel topMenuTreeItem = DocumentTreeModelsExtensions.CreateKendoTreeViewModel(topMenuItem);
                var children = DocumentTreeModelsExtensions.GetChildItems(documents, topMenuItem.Id);
                if (children != null && children.Any())
                {
                    foreach (var child in children)
                    {
                        var subTopTree = DocumentTreeModelsExtensions.CreateKendoTreeViewModel(child);
                        topMenuTreeItem.Items.Add(subTopTree);
                        var subchildren = DocumentTreeModelsExtensions.GetChildItems(documents, child.Id);
                        if (subchildren == null || !subchildren.Any()) continue;
                        foreach (var subchild in subchildren)
                        {
                            var subsubTopTree = DocumentTreeModelsExtensions.CreateKendoTreeViewModel(subchild);
                            subTopTree.Items.Add(subsubTopTree);
                            var subsubchildren = DocumentTreeModelsExtensions.GetChildItems(documents, subchild.Id);
                            if (subsubchildren == null || !subsubchildren.Any()) continue;

                            foreach (var subsubchild in subsubchildren)
                            {
                                var subsubTreechild = DocumentTreeModelsExtensions.CreateKendoTreeViewModel(subsubchild);
                                subsubTopTree.Items.Add(subsubTreechild);
                            }
                        }
                    }
                }
                result.Add(topMenuTreeItem);
            }
            return result;
        }

        public static List<TreeViewItemModel> ToKendoTreeViewItemModelList_todo(this DocumentTreeModels documents)
        {
            List<Kendo.Mvc.UI.TreeViewItemModel> result = null;
            if (documents?.Documents == null || !documents.Documents.Any()) return null;

            var topMenuItems = DocumentTreeModelsExtensions.GetChildItems(documents, 0);

            result = new List<TreeViewItemModel>();

            foreach (var topMenuItem in topMenuItems)
            {
                TreeViewItemModel topMenuTreeItem = DocumentTreeModelsExtensions.CreateKendoTreeViewModel(topMenuItem);
                result.Add(topMenuTreeItem);

                if (!topMenuItem.IsDocument)
                {
                    var children = DocumentTreeModelsExtensions.GetChildItems(documents, topMenuItem.Id);
                    if (children != null && children.Any())
                    {
                        DocumentTreeModelsExtensions.ProcessSubDocuments(documents, children, ref topMenuTreeItem);
                    }
                }
            }
            return result;
        }
        
        private static void ProcessSubDocuments(DocumentTreeModels documents, List<DocumentTreeModel> children, ref TreeViewItemModel parent)
        {
            foreach (var child in children)
            {
                var item = DocumentTreeModelsExtensions.CreateKendoTreeViewModel(child);
                parent.Items.Add(DocumentTreeModelsExtensions.CreateKendoTreeViewModel(child));
                if (!child.IsDocument)
                {
                    var subchildren = DocumentTreeModelsExtensions.GetChildItems(documents, child.Id);
                    if (subchildren == null || !subchildren.Any()) continue;
                    DocumentTreeModelsExtensions.ProcessSubDocuments(documents, subchildren, ref item);
                }
            }
        }

        private static List<DocumentTreeModel> GetTopLevelItems(DocumentTreeModels documents, int documentId)
        {
            return documents.Documents.FindAll(m => m.ParentId == documentId || m.ParentId > 0 && documents.Documents.Count(s => s.Id == m.ParentId) == 0);
        }

        private static List<DocumentTreeModel> GetChildItems(DocumentTreeModels documents, int documentId)
        {
            return documents.Documents.FindAll(m => m.ParentId == documentId);
        }

        private static TreeViewItemModel CreateKendoTreeViewModel(DocumentTreeModel document)
        {
            return new TreeViewItemModel { Text = document.Name, Id = document.Id.ToString(), Items = new List<TreeViewItemModel>() };
        }
    }
}
