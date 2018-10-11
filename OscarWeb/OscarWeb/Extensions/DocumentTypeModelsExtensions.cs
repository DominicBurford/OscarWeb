using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

using Common.Models;

namespace OscarWeb.Extensions
{
    /// <summary>
    /// DocumentTypeModels extensions.
    /// </summary>
    public static class DocumentTypeModelsExtensions
    {
        /// <summary>
        /// Converts an instance of <see cref="DocumentTypeModels"/> into a list of <see cref="SelectListItem"/>
        /// </summary>
        /// <param name="documentTypes"></param>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectListItems(this DocumentTypeModels documentTypes)
        {
            if (documentTypes?.DocumentTypes == null || !documentTypes.DocumentTypes.Any()) return null;

            return documentTypes.DocumentTypes.Select(documentType => new SelectListItem
                {
                    Value = documentType.Id.ToString(),
                    Text = $"{documentType.Description} ({documentType.Extension})"
                })
                .ToList();
        }
    }
}
