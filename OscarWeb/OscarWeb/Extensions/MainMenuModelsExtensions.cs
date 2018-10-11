using System.Collections.Generic;
using System.Linq;

using OscarWeb.Models;

using Common.Models;
using Kendo.Mvc.UI;

namespace OscarWeb.Extensions
{
    /// <summary>
    /// Extensions for <see cref="MainMenuModels"/>
    /// </summary>
    public static class MainMenuModelsExtensions
    {
        /// <summary>
        /// Convert a menu of type <see cref="MainMenuModels"/> into a tree structure of type <see cref="MenuTreeViewItemModel"/>
        /// as used by the Kendo UI tree component.
        /// </summary>
        /// <param name="mainmenu">An instance of <seealso cref="MainMenuModels"/></param>
        /// <returns>A list of <see cref="MenuTreeViewItemModel"/></returns>
        public static List<MenuTreeViewItemModel> ToTreeViewItemModelList(this MainMenuModels mainmenu)
        {
            List<MenuTreeViewItemModel> result = null;
            if (mainmenu?.MenuItems == null || !mainmenu.MenuItems.Any()) return null;

            var topMenuItems = MainMenuModelsExtensions.GetChildItems(mainmenu, 0);

            result = new List<MenuTreeViewItemModel>();

            foreach (var topMenuItem in topMenuItems)
            {
                MenuTreeViewItemModel topMenuTreeItem = MainMenuModelsExtensions.CreateTreeViewModel(topMenuItem);
                var children = MainMenuModelsExtensions.GetChildItems(mainmenu, topMenuItem.Id);
                if (children != null && children.Any())
                {
                    foreach (var child in children)
                    {
                        var subTopTree = MainMenuModelsExtensions.CreateTreeViewModel(child);
                        topMenuTreeItem.Items.Add(subTopTree);
                        var subchildren = MainMenuModelsExtensions.GetChildItems(mainmenu, child.Id);
                        if (subchildren == null || !subchildren.Any()) continue;
                        foreach (var subchild in subchildren)
                        {
                            subTopTree.Items.Add(MainMenuModelsExtensions.CreateTreeViewModel(subchild));
                        }
                    }
                }
                result.Add(topMenuTreeItem);
            }
            return result;
        }

        /// <summary>
        /// Convert a menu of type <see cref="MainMenuModels"/> into a tree structure of type <see cref="TreeViewItemModel"/>
        /// as used by the Kendo UI tree component.
        /// </summary>
        /// <param name="mainmenu">An instance of <seealso cref="MainMenuModels"/></param>
        /// <returns>A list of <see cref="TreeViewItemModel"/></returns>
        public static List<TreeViewItemModel> ToKendoTreeViewItemModelList(this MainMenuModels mainmenu)
        {
            List<Kendo.Mvc.UI.TreeViewItemModel> result = null;
            if (mainmenu?.MenuItems == null || !mainmenu.MenuItems.Any()) return null;

            var topMenuItems = MainMenuModelsExtensions.GetChildItems(mainmenu, 0);

            result = new List<TreeViewItemModel>();

            foreach (var topMenuItem in topMenuItems)
            {
                TreeViewItemModel topMenuTreeItem = MainMenuModelsExtensions.CreateKendoTreeViewModel(topMenuItem);
                var children = MainMenuModelsExtensions.GetChildItems(mainmenu, topMenuItem.Id);
                if (children != null && children.Any())
                {
                    foreach (var child in children)
                    {
                        var subTopTree = MainMenuModelsExtensions.CreateKendoTreeViewModel(child);
                        topMenuTreeItem.Items.Add(subTopTree);
                        var subchildren = MainMenuModelsExtensions.GetChildItems(mainmenu, child.Id);
                        if (subchildren == null || !subchildren.Any()) continue;
                        foreach (var subchild in subchildren)
                        {
                            subTopTree.Items.Add(MainMenuModelsExtensions.CreateKendoTreeViewModel(subchild));
                        }
                    }
                }
                result.Add(topMenuTreeItem);
            }
            return result;
        }

        /// <summary>
        /// Convert a menu of type <see cref="MainMenuModels"/> into a menu structure of type <see cref="MenuItem"/>
        /// as used by the Kendo UI menu component.
        /// </summary>
        /// <param name="mainmenu">An instance of <seealso cref="MainMenuModels"/></param>
        /// <returns>A list of <see cref="MenuItem"/></returns>
        public static List<MenuItem> ToKendoMenuItemModelList(this MainMenuModels mainmenu)
        {
            List<MenuItem> result = null;
            if (mainmenu?.MenuItems == null || !mainmenu.MenuItems.Any()) return null;

            var topMenuItems = MainMenuModelsExtensions.GetChildItems(mainmenu, 0);

            result = new List<MenuItem>();

            foreach (var topMenuItem in topMenuItems)
            {
                MenuItem topMenuTreeItem = MainMenuModelsExtensions.CreateKendoMenuViewModel(topMenuItem);
                var children = MainMenuModelsExtensions.GetChildItems(mainmenu, topMenuItem.Id);
                if (children != null && children.Any())
                {
                    foreach (var child in children)
                    {
                        var subTopTree = MainMenuModelsExtensions.CreateKendoMenuViewModel(child);
                        topMenuTreeItem.Items.Add(subTopTree);
                        var subchildren = MainMenuModelsExtensions.GetChildItems(mainmenu, child.Id);
                        if (subchildren == null || !subchildren.Any()) continue;
                        foreach (var subchild in subchildren)
                        {
                            subTopTree.Items.Add(MainMenuModelsExtensions.CreateKendoMenuViewModel(subchild));
                        }
                    }
                }
                result.Add(topMenuTreeItem);
            }
            return result;
        }
        
        private static List<MainMenuModel> GetChildItems(MainMenuModels menuitems, int menuId)
        {
            return menuitems.MenuItems.FindAll(m => m.ParentId == menuId);
        }

        private static MenuTreeViewItemModel CreateTreeViewModel(MainMenuModel menuitem)
        {
            return new MenuTreeViewItemModel {Text = menuitem.DisplayText, Items = new List<MenuTreeViewItemModel>()};
        }

        private static TreeViewItemModel CreateKendoTreeViewModel(MainMenuModel menuitem)
        {
            return new TreeViewItemModel { Text = menuitem.DisplayText, Id = menuitem.Id.ToString(), Items = new List<TreeViewItemModel>(), Url = menuitem.Routing };
        }
        
        private static MenuItem CreateKendoMenuViewModel(MainMenuModel menuitem)
        {
            return new MenuItem { Text = menuitem.DisplayText, Url = menuitem.Routing, Enabled = true, ImageUrl = menuitem.Icon };
        }
    }
}
