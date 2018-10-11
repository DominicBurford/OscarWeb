using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Common.Models;
using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;

namespace OscarWeb.ViewComponents
{
    public class SubFormMenuViewComponent : ViewComponent
    {
        /// <summary>
        /// Creates a sub-form-level menu blade
        /// </summary>
        /// <param name="parentId">The parent ID of the menu item</param>
        /// <param name="menuname">The name of the menu as stored in HTTP session storage</param>
        /// <param name="formmenu">An instance of <see cref="MainMenuModels"/> to populate</param>
        /// <param name="context">The context session - used for mocking the HTTP context for unit testing.</param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(int parentId, string menuname, MainMenuModels formmenu, ISession context = null)
        {
            //for unit-testing we pass in an instance of HttpContext.Session to allow for mocking the environment
            if (context == null)
            {
                context = HttpContext.Session;
            }

            if (context.Get<MainMenuModels>(menuname) == null)
            {
                LoggingService service = new LoggingService();
                var stopwatch = new Stopwatch();

                try
                {
                    formmenu = await new MenuServices().GetModulesItemsForUser(
                        context.Get<string>(SessionConstants.EmailClaim),
                        parentId,
                        context.Get<string>(SessionConstants.WebServicesUrl),
                        context.Get<string>(SessionConstants.EncodedUserId));
                    context.Set<MainMenuModels>(menuname, formmenu);
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
                    service.TrackEvent(LoggingServiceConstants.GetModulesItemsForUser, stopwatch.Elapsed, properties);
                }
            }
            else
            {
                formmenu = context.Get<MainMenuModels>(menuname);
            }
            return View(formmenu);
        }
    }
}
