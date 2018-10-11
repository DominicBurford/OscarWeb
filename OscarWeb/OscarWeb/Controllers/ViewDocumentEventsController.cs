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
    public class ViewDocumentEventsController : BaseGridController
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
            int documentId = base.GetSession().Get<int>(SessionConstants.CurrentDocumentId);
            string document = documentId == 0 ? null : documentId.ToString();
            
            var result = new List<DocumentEventModel>();

            var documentEvents = await new DocumentManagerPageModelService().GetDocumentEvents(user.CompanyId.ToString(), null, document, rootUrl, encodedId);
            if (documentEvents != null && documentEvents.DocumentEvents.Any())
            {
                result = documentEvents.DocumentEvents;
            }
            return Json(result.ToDataSourceResult(request));
        }
    }
}