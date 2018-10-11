using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;

using Common.Models;
using Kendo.Mvc.UI;

namespace OscarWeb.ViewComponents
{
    /// <summary>
    /// View component responsible for creating the main menu navigation
    /// using a Kendo menu component
    /// </summary>
    public class KendoMenuViewComponent : ViewComponent
    {
        /// <summary>
        /// Fetches the required menu structure using a service thyen maps this into a Kendo UI tree structure.
        /// </summary>
        /// <param name="mainmenu"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(List<MenuItem> mainmenu, ISession context = null)
        {
            //for unit-testing we pass in an instance of HttpContext.Session to allow for mocking the environment
            if (context == null)
            {
                context = HttpContext.Session;
            }

            var menuitems = context.Get<List<MenuItem>>(SessionConstants.KendoMainMenu);
            if (menuitems == null)
            {
                LoggingService service = new LoggingService();
                var stopwatch = new Stopwatch();

                try
                {
                    List<MenuItem> treemenu = null;
                    var response = await new MenuServices().GetModulesItemsForUser(
                        context.Get<string>(SessionConstants.EmailClaim),
                        context.Get<string>(SessionConstants.WebServicesUrl),
                        context.Get<string>(SessionConstants.EncodedUserId));

                    response = new MenuServices().CleanMainMenuModel(response, context.Get<UserModel>(SessionConstants.CurrentUser));
                    treemenu = response == null ? KendoMenuViewComponent.GetEmptyTreeMenu() : response.ToKendoMenuItemModelList();
                    context.Set<List<MenuItem>>(SessionConstants.KendoMainMenu, treemenu);
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
                    service.TrackEvent(LoggingServiceConstants.GetKendoMainMenuItems, stopwatch.Elapsed, properties);
                }
            }
            ViewData[SessionConstants.ViewKendoMainMenu] =
                context.Get<List<MenuItem>>(SessionConstants.KendoMainMenu);

            return View(context.Get<List<MenuItem>>(SessionConstants.KendoMainMenu));
        }

        /// <summary>
        /// If the user has not been setup in OscarWeb then display an empty menu tree
        /// </summary>
        /// <returns></returns>
        private static List<MenuItem> GetEmptyTreeMenu()
        {
            var result = new List<MenuItem>();
            MenuItem emptymenuitem = new MenuItem
            {
                Text = StringConstants.NoItemsToDisplay
            };
            result.Add(emptymenuitem);
            return result;
        }
    }
}
