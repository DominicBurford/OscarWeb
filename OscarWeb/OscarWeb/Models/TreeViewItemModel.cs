using System.Collections.Generic;

namespace OscarWeb.Models
{
    public class MenuTreeViewItemModel
    {
        public  string Text { get; set; }
        public List<MenuTreeViewItemModel> Items { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MenuTreeViewItemModel()
        {
            this.Items = new List<MenuTreeViewItemModel>();
        }
    }
}
