using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using OscarWeb.Constants;

using Common.Models;
using Kendo.Mvc.UI;

namespace OscarWeb.Services
{
    /// <summary>
    /// Class responsible for all user related requests
    /// </summary>
    public class UserServices
    {
        /// <summary>
        /// Retrieve the specified user 
        /// </summary>
        /// <param name="useremail"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<UserModel> GetUserDetails(string useremail, string rooturl)
        {
            if (string.IsNullOrEmpty(useremail) || string.IsNullOrEmpty(rooturl)) return null;

            UserModel result = null;
            string url = $"{rooturl}api/webtasks?username={useremail}";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url);
                if (!string.IsNullOrEmpty(response) && response.Length > 0)
                {
                    result = new SerializerServices().DeserializeObject<UserModel>(response);
                }
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
                    { "UserEmail", useremail },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.GetUserDetailsService, stopwatch.Elapsed, properties);
            }
            return result;
        }
    }
}
