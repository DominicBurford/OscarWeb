using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using OscarWeb.Constants;

using Common.Constants;
using Common.Models.EmailRequests;

namespace OscarWeb.Services
{
    public class SendEmailService
    {
        /// <summary>
        /// Send an email document subscription notification
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> SendDocumentNotification(DocumentSubscriberNotificationModel notification, string rooturl)
        {
            bool result = false;

            if (notification == null || string.IsNullOrEmpty(notification.UserName) || string.IsNullOrEmpty(notification.DocumentName) || string.IsNullOrEmpty(rooturl)) return false;

            string url = $"{rooturl}api/sendemail?emailname={RoutingTasksTypeConstants.DocumentNotification}";
            string payload = new SerializerServices().SerializeObject(notification);

            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().PostData(url, content);
                result = response.IsSuccessStatusCode;
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
                    { "UserName", notification.UserName },
                    { "WebServicesEndpoint", rooturl },
                    { "DocumentName", notification.DocumentName }
                };
                service.TrackEvent(LoggingServiceConstants.SendDocumentNotification, stopwatch.Elapsed, properties);
            }
            return result;
        }
    }
}
