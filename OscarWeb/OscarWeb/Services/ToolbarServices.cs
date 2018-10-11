using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using OscarWeb.Constants;
using OscarWeb.Extensions;

using Common.Constants;
using Common.Models;

namespace OscarWeb.Services
{
    /// <summary>
    /// Class responsible for retrieving the toolbars
    /// </summary>
    public class ToolbarServices
    {
        /// <summary>
        /// Retrieve the specified toolbar by its name
        /// </summary>
        /// <param name="useremail"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="toolbarname"></param>
        public async Task<ToolbarModel> GetToolbar(string useremail, string rooturl, string encodedId, string toolbarname)
        {
            ToolbarModel result = null;

            if (string.IsNullOrEmpty(useremail) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) ||
                string.IsNullOrEmpty(toolbarname)) return null;

            string queryname = WebTasksTypeConstants.GetToolbarByName;
            string queryterms = WebApiServices.GetJsonQuerySearchTermsToolbarname(useremail, toolbarname);
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response) && response.Length > 0)
                {
                    result = new SerializerServices().DeserializeObject<ToolbarModel>(response.NormalizeJsonString());
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
                    { "WebServicesEndpoint", rooturl },
                    { "EncodedId", encodedId },
                    { "ToolbarName", toolbarname }
                };
                service.TrackEvent(LoggingServiceConstants.GetToolbar, stopwatch.Elapsed, properties);
            }
            return result;
        }
    }
}
