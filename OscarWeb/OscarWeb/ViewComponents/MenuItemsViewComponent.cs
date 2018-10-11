using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Common.Models;
using Microsoft.AspNetCore.Http;
using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;

namespace OscarWeb.ViewComponents
{
    /// <summary>
    /// View component responsible for creating the menu tree structure
    /// </summary>
    public class MenuItemsViewComponent: ViewComponent
    {
        /// <summary>
        /// Fetch the required menu items for the specified menu level
        /// </summary>
        /// <param name="parentId">The ID of the parent menu item</param>
        /// <param name="context">An optional instance of <see cref="ISession"/> to allow the view component to be unit tested</param>
        /// <returns>The view for the menu tree structure</returns>
        public async Task<IViewComponentResult> InvokeAsync(int parentId, ISession context = null)
        {
            //for unit-testing we pass in an instance of HttpContext.Session to allow for mocking the environment
            if (context == null)
            {
                context = HttpContext.Session;
            }

            var menuitems = context.Get<MainMenuModels>(SessionConstants.TopLevelMenuItems);
            if (menuitems == null)
            {
                LoggingService service = new LoggingService();
                var stopwatch = new Stopwatch();

                try
                {
                    var response = await new MenuServices().GetModulesItemsForUser(
                        context.Get<string>(SessionConstants.EmailClaim),
                        parentId,
                        context.Get<string>(SessionConstants.WebServicesUrl),
                        context.Get<string>(SessionConstants.EncodedUserId));

                    response = new MenuServices().CleanMainMenuModel(response, context.Get<UserModel>(SessionConstants.CurrentUser));
                    context.Set<MainMenuModels>(SessionConstants.TopLevelMenuItems, response);
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
                    service.TrackEvent(LoggingServiceConstants.GetTopLevelModules, stopwatch.Elapsed, properties);
                }
            }
            return View(context.Get<MainMenuModels>(SessionConstants.TopLevelMenuItems));
        }
    }
}