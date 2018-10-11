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
    public class RoleAdminPageModelService : PageModelService
    {
        public override string ModuleName { get; }

        public RoleAdminPageModelService()
        {
            ModuleName = ModuleNameConstants.RoleAdmin;
        }

        /// <summary>
        /// Retrieves all the roles for the specified company
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<RoleModels> GetRolesByCompanyId(string email, string rooturl, string encodedId, int companyId)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) || companyId <= 0) return null;

            string queryname = WebTasksTypeConstants.GetRolesByCompanyId;
            string queryterms = WebApiServices.GetCompanyIdJsonQuerySearchTerms(companyId.ToString());
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";
            RoleModels result = null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<RoleModels>(response.NormalizeJsonString());
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
                service.TrackEvent(LoggingServiceConstants.GetRolesByCompanyId, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="role"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> CreateRole(RoleModel role, string encodedId, string rooturl)
        {
            if (string.IsNullOrEmpty(role?.Name) || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result;
            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.AddRole}&useRoutingController=true";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string payload = new SerializerServices().SerializeObject(role);
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
                    { "Rolename", role.Name },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.CreateRole, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Update a role
        /// </summary>
        /// <param name="role"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRole(RoleModel role, string encodedId, string rooturl)
        {
            if (string.IsNullOrEmpty(role?.Name) || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result;
            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.UpdateRole}&useRoutingController=true";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string payload = new SerializerServices().SerializeObject(role);
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
                    { "Rolename", role.Name },
                    { "RoleId", role.Id.ToString() },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.UpdateRole, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="role"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRole(RoleModel role, string encodedId, string rooturl)
        {
            if (string.IsNullOrEmpty(role?.Name) || role.Id < 0 || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result;
            string queryterms = WebApiServices.GetRoleIdJsonQuerySearchTerms(role.Id.ToString());

            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.DeleteRole}&queryterms={queryterms}";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string payload = new SerializerServices().SerializeObject(role);
                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
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
                    { "Rolename", role.Name },
                    { "RoleId", role.Id.ToString() },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.DeleteRole, stopwatch.Elapsed, properties);
            }
            return result;
        }
    }
}
