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
    public class CompanyAdminPageModelService : PageModelService
    {
        public CompanyAdminPageModelService()
        {
            ModuleName = ModuleNameConstants.CompanyAdmin;
        }

        public override string ModuleName { get; }

        /// <summary>
        /// Retrieves a list of companies
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <returns></returns>
        public async Task<CompanyModels> GetAllCompanies(string email, string rooturl, string encodedId)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId)) return null;

            string queryname = WebTasksTypeConstants.GetAllCompanies;
            string queryterms = WebApiServices.GetEmailJsonQuerySearchTerms(email);
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";
            CompanyModels result = null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<CompanyModels>(response.NormalizeJsonString());
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
                    { "EncodedId", encodedId }
                };
                service.TrackEvent(LoggingServiceConstants.GetAllCompanies, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Create a new company
        /// </summary>
        /// <param name="company"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> CreateCompany(CompanyModel company, string encodedId, string rooturl)
        {
            if (string.IsNullOrEmpty(company?.Name) || string.IsNullOrEmpty(company.StorageContainerName) || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result; 
            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.AddCompany}&useRoutingController=true";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string payload = new SerializerServices().SerializeObject(company);
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
                    { "CompanyName", company.Name },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.CreateCompany, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Update the specified company
        /// </summary>
        /// <param name="company"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> UpdateCompany(CompanyModel company, string encodedId, string rooturl)
        {
            if (string.IsNullOrEmpty(company?.Name) || string.IsNullOrEmpty(company.StorageContainerName) || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result;
            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.UpdateCompany}&useRoutingController=true";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string payload = new SerializerServices().SerializeObject(company);
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
                    { "CompanyName", company.Name },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.UpdateCompany, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Delete the specified company
        /// </summary>
        /// <param name="name"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCompany(string name, string encodedId, string rooturl)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result;
            string queryterms = WebApiServices.GetCompanyNameJsonQuerySearchTerms(name);

            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.DeleteCompany}&queryterms={queryterms}";

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
                    { "CompanyName", name },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.DeleteCompany, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Retrieves a company by its ID
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<CompanyModel> GetCompanyById(string email, string rooturl, string encodedId, int companyId)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) || companyId <= 0) return null;

            string queryname = WebTasksTypeConstants.GetCompanyById;
            string queryterms = WebApiServices.GetCompanyIdJsonQuerySearchTerms(companyId.ToString());
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";
            CompanyModel result = null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<CompanyModel>(response.NormalizeJsonString());
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
                service.TrackEvent(LoggingServiceConstants.GetCompanyById, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Create a new company address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> CreateCompanyAddress(CompanyAddressModel address, string encodedId, string rooturl)
        {
            if (string.IsNullOrEmpty(address?.Postcode) || string.IsNullOrEmpty(address.AddressLine1) || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result;
            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.AddCompanyAddress}&useRoutingController=true";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string payload = new SerializerServices().SerializeObject(address);
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
                    { "CompanyAddress", $"{address.Postcode} {address.AddressLine1}" },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.CreateCompanyAddress, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Update the specified company address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> UpdateCompanyAddress(CompanyAddressModel address, string encodedId, string rooturl)
        {
            if (string.IsNullOrEmpty(address?.Postcode) || string.IsNullOrEmpty(address.AddressLine1) || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result;
            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.UpdateCompanyAddress}&useRoutingController=true";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string payload = new SerializerServices().SerializeObject(address);
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
                    { "CompanyAddress", $"{address.Postcode} {address.AddressLine1}" },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.UpdateCompanyAddress, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Delete the specified company
        /// </summary>
        /// <param name="id"></param>
        /// <param name="encodedId"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCompanyAddress(int id, string encodedId, string rooturl)
        {
            if (id <= 0 || string.IsNullOrEmpty(encodedId) || string.IsNullOrEmpty(rooturl)) return false;

            bool result;
            string queryterms = WebApiServices.GetJsonQuerySearchTermsCompanyAddressId(id.ToString());

            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.DeleteCompanyAddress}&queryterms={queryterms}";

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
                    { "CompanyAddressId", id.ToString() },
                    { "EncodedId", encodedId },
                    { "WebServicesEndpoint", rooturl }
                };
                service.TrackEvent(LoggingServiceConstants.DeleteCompanyAddress, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Return company address(es)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="companyId"></param>
        /// <param name="returnAll"></param>
        /// <returns></returns>
        public async Task<CompanyAddressModels> GetCompanyAddresses(string email, string rooturl, string encodedId, int companyId, bool returnAll)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) || companyId <= 0) return null;

            string queryname = WebTasksTypeConstants.GetCompanyAddresses;
            string queryterms = WebApiServices.GetJsonQuerySearchTermsCompanyAddresses(companyId, returnAll);
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";
            CompanyAddressModels result = null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<CompanyAddressModels>(response.NormalizeJsonString());
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
                    { "CompanyId", companyId.ToString() },
                    { "ReturnAll", returnAll.ToString() }
                };
                service.TrackEvent(LoggingServiceConstants.GetCompanyAddresses, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Return a company address
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CompanyAddressModel> GetCompanyAddress(string email, string rooturl, string encodedId, int id)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) || id <= 0) return null;

            string queryname = WebTasksTypeConstants.GetCompanyAddress;
            string queryterms = WebApiServices.GetJsonQuerySearchTermsCompanyAddressId(id.ToString());
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";
            CompanyAddressModel result = null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<CompanyAddressModel>(response.NormalizeJsonString());
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
                    { "CompanyAddressId", id.ToString() }
                };
                service.TrackEvent(LoggingServiceConstants.GetCompanyAddress, stopwatch.Elapsed, properties);
            }
            return result;
        }
    }
}
