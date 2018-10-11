using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

using Common.Models;

namespace OscarWeb.Extensions
{
    /// <summary>
    /// DocumentCategoryModels extensions.
    /// </summary>
    public static class DocumentCategoryModelsExtensions
    {
        /// <summary>
        /// Convert an instance of <see cref="DocumentCategoryModels"/> into a list of <see cref="SelectListItem"/>
        /// </summary>
        /// <param name="documentCategories"></param>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectListItems(this DocumentCategoryModels documentCategories)
        {
            if (documentCategories?.DocumentCategories == null || !documentCategories.DocumentCategories.Any()) return null;

            return documentCategories.DocumentCategories.Select(documentType => new SelectListItem
                {
                    Value = documentType.Id.ToString(),
                    Text = documentType.Description
                })
                .ToList();
        }
    }
}
