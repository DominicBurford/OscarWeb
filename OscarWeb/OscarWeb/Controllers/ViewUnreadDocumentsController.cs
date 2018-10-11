using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;

using Common.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace OscarWeb.Controllers
{
    public class ViewUnreadDocumentsController : BaseGridController
    {
        /// <summary>
        /// Populate the grid with data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ActionResult> EditingPopup_Read([DataSourceRequest] DataSourceRequest request)
        {
            string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);
            UserModel user = base.GetSession().Get<UserModel>(SessionConstants.CurrentUser);
            
            var result = new List<DocumentUnreadModel>();

            var documents = await new DocumentManagerPageModelService().GetUnreadDocuments(user.Email, rootUrl, encodedId);
            if (documents?.Documents != null && documents.Documents.Any())
            {
                result = documents.Documents;
            }
            return Json(result.ToDataSourceResult(request));
        }
    }
}