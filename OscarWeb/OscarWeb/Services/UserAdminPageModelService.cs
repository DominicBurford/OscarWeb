using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using OscarWeb.Constants;
using OscarWeb.Extensions;

using Common.Constants;
using Common.Models;

namespace OscarWeb.Services
{
    public class UserAdminPageModelService : PageModelService
    {
        public override string ModuleName { get; }

        public UserAdminPageModelService()
        {
            ModuleName = ModuleNameConstants.UserAdmin;
        }

        /// <summary>
        /// Retrieves all the users for the specified company
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<UserModels> GetUsersByCompanyId(string email, string rooturl, string encodedId, int companyId)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) || companyId <= 0) return null;

            string queryname = WebTasksTypeConstants.GetUsersByCompanyId;
            string queryterms = WebApiServices.GetCompanyIdJsonQuerySearchTerms(companyId.ToString());
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";
            UserModels result = null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<UserModels>(response.NormalizeJsonString());
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
                    { "UserEmail", email },
                    { "WebServicesEndpoint", rooturl },
                    { "EncodedId", encodedId },
                    { "CompanyId", companyId.ToString() }
                };
                service.TrackEvent(LoggingServiceConstants.GetUsersByCompanyId, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Update the specified user 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserDetails(UserModel user, string encodedId, string rooturl)
        {
            if (string.IsNullOrEmpty(user?.Email) || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result;
            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.UpdateUser}&useRoutingController=true";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string payload = new SerializerServices().SerializeObject(user);
                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await new WebApiServices().PutData(url, content, encodedId);
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
                    { "UserEmail", user.Email },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.UpdateUserDetails, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Create a new user user 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> CreateUser(UserModel user, string encodedId, string rooturl)
        {
            if (string.IsNullOrEmpty(user?.Email) || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result;
            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.AddUser}&useRoutingController=true";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string payload = new SerializerServices().SerializeObject(user);
                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await new WebApiServices().PostData(url, content, encodedId);
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
                    { "UserEmail", user.Email },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.CreateUser, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Delete the specified user 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUser(string email, string encodedId, string rooturl)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result;
            string queryterms = WebApiServices.GetEmailJsonQuerySearchTerms(email);

            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.DeleteUser}&queryterms={queryterms}";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().DeleteData(url, encodedId);
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
                    { "UserEmail", email },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.DeleteUser, stopwatch.Elapsed, properties);
            }
            return result;
        }
    }
}
