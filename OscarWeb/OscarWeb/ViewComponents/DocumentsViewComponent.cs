using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;

using Kendo.Mvc.UI;

namespace OscarWeb.ViewComponents
{
    /// <summary>
    /// View component responsible for creating the document navigation
    /// using a Kendo UI tree component
    /// </summary>
    public class DocumentsViewComponent : ViewComponent
    {
        /// <summary>
        /// Fetches the required document structure using a service then maps this into a Kendo UI tree structure.
        /// </summary>
        /// <param name="mainmenu"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(List<TreeViewItemModel> mainmenu, ISession context = null)
        {
            //for unit-testing we pass in an instance of HttpContext.Session to allow for mocking the environment
            if (context == null)
            {
                context = HttpContext.Session;
            }

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new DocumentManagerPageModelService().GetDocumentsForDocumentTree(
                    context.Get<string>(SessionConstants.EmailClaim),
                    context.Get<string>(SessionConstants.WebServicesUrl),
                    context.Get<string>(SessionConstants.EncodedUserId));

                List<TreeViewItemModel> treemenu = null;
                if (response?.Documents != null && response.Documents.Any())
                {
                    treemenu = response.ToKendoTreeViewItemModelList();
                }
                else
                {
                    treemenu = new List<TreeViewItemModel>();
                }
                context.Set<List<TreeViewItemModel>>(SessionConstants.DocumentTreeMenu, treemenu);
            }
            catch (Exception ex)
            {
                service.TrackException(ex);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                var properties = new Dictionary<string, string>
                {
                    { "UserEmail", context.Get<string>(SessionConstants.EmailClaim) },
                    { "WebServicesEndpoint", context.Get<string>(SessionConstants.WebServicesUrl) },
                    { "EncodedId", context.Get<string>(SessionConstants.EncodedUserId) }
                };
                service.TrackEvent(LoggingServiceConstants.GetDocumentTree, stopwatch.Elapsed, properties);
            }

            ViewData[SessionConstants.ViewDocumentTreeMenu] =
                context.Get<List<TreeViewItemModel>>(SessionConstants.DocumentTreeMenu);

            return View(context.Get<List<TreeViewItemModel>>(SessionConstants.DocumentTreeMenu));
        }
    }
}
