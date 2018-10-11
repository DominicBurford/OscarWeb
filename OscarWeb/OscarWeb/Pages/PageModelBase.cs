using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;
using OscarWeb.Models;

using Common.Models;

namespace OscarWeb.Pages
{
    /// <summary>
    /// Base class for all Razor page model classes. 
    /// Provides separation of concerns between the Razor page (UI) and its underlying business logic. 
    /// </summary>
    /// <typeparam name="T">The type of the business service class which must be inherited from <see cref="PageModelService"/></typeparam>
    public class PageModelBase<T>: PageModel where T : PageModelService, new()
    {
        public readonly T Service = new T();

        private IOptions<WebServicesModel> WebServicesUrl { get; set; }

        public PageModelBase(IOptions<WebServicesModel> webServicesUrl)
        {
            this.WebServicesUrl = webServicesUrl;
        }

        /// <summary>
        /// Sets the session variables. These are derived from the User Principal. Once the user
        /// has been established then the session variables can be set. These include the user's email address and ID.
        /// </summary>
        /// <returns></returns>
        public async Task SetSession()
        {
            LoggingService service = new LoggingService();

            if (User.Identity.IsAuthenticated)
            {
                //save the user's email and whether they are a new user to session storage
                Claim emailClaim = User.Claims.FirstOrDefault(claim => claim.Type == SessionConstants.EmailClaim);
                Claim isNewClaim = User.Claims.FirstOrDefault(claim => claim.Type == SessionConstants.IsNewUserClaim);

                if (emailClaim != null)
                {
                    HttpContext.Session.Set<string>(SessionConstants.EmailClaim, emailClaim.Value);
                    if (isNewClaim != null)
                    {
                        HttpContext.Session.Set<string>(SessionConstants.EmailClaim, isNewClaim.Value);
                    }

                    //check to see if the user details have been retrieved
                    if (HttpContext.Session.Get<int>(SessionConstants.CurrentUserId) == 0)
                    {
                        UserModel user = null;
                        var stopwatch = new Stopwatch();
                        try
                        {
                            //retrieve the user info
                            service.TrackTrace($"Fetching user details for {emailClaim.Value}");
                            user =
                                await new UserServices().GetUserDetails(emailClaim.Value,
                                    this.WebServicesUrl.Value.Endpoint);
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
                                { "UserEmail", emailClaim.Value },
                                { "WebServicesEndpoint", this.WebServicesUrl.Value.Endpoint }
                            };
                            service.TrackEvent(LoggingServiceConstants.GetUserDetails, stopwatch.Elapsed, properties);
                        }

                        //de-serialise the response and store the User ID
                        if (user != null && user.Id > 0 && string.CompareOrdinal(user.Email, emailClaim.Value) == 0)
                        {
                            //store user info to future requests
                            HttpContext.Session.Set<int>(SessionConstants.CurrentUserId, user.Id);
                            HttpContext.Session.Set<string>(SessionConstants.EncodedUserId, user.Id.ToString().Base64Encode());
                            HttpContext.Session.Set<UserModel>(SessionConstants.CurrentUser, user);
                            HttpContext.Session.Set<string>(SessionConstants.WebServicesUrl, this.WebServicesUrl.Value.Endpoint);
                        }
                    }
                }
            }
        }
    }
}
