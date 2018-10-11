using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using OscarWeb.Constants;

namespace OscarWeb.Services
{
    /// <summary>
    /// The class responsible for all GET, PUT, POST and DELETE HTTP service calls
    /// </summary>
    public class WebApiServices
    {
        /// <summary>
        /// Posts HTTP data to the specified endpoint
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="encodedId">The user's base64 encoded user ID</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostData(string url, HttpContent content, string encodedId)
        {
            string token = JsonWebTokenServices.CreateJsonWebtoken();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(WebServiceConstants.Subscriber, token);
                client.DefaultRequestHeaders.Add("WebRequestUserID", encodedId);
                return await client.PostAsync(new Uri(url), content);
            }
        }

        /// <summary>
        /// Posts HTTP data to the specified endpoint
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostData(string url, HttpContent content)
        {
            string token = JsonWebTokenServices.CreateJsonWebtoken();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(WebServiceConstants.Subscriber, token);
                return await client.PostAsync(new Uri(url), content);
            }
        }

        /// <summary>
        /// Retrieves data from the specified endpoint
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encodedId">The user's base64 encoded user ID.
        /// N.B. Not needed when initially retrieveing the user's information.</param>
        /// <returns></returns>
        public async Task<string> GetData(string url, string encodedId = null)
        {
            using (var client = new HttpClient())
            {
                string token = JsonWebTokenServices.CreateJsonWebtoken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(WebServiceConstants.Subscriber, token);
                if (!string.IsNullOrEmpty(encodedId))
                {
                    client.DefaultRequestHeaders.Add("WebRequestUserID", encodedId);
                }
                using (var r = await client.GetAsync(new Uri(url)))
                {
                    string result = await r.Content.ReadAsStringAsync();
                    return result;
                }
            }
        }

        /// <summary>
        /// Delete the specified resource
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encodedId">The user's base64 encoded user ID</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteData(string url, string encodedId)
        {
            using (var client = new HttpClient())
            {
                string token = JsonWebTokenServices.CreateJsonWebtoken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(WebServiceConstants.Subscriber, token);
                client.DefaultRequestHeaders.Add("WebRequestUserID", encodedId);
                return await client.DeleteAsync(new Uri(url));
            }
        }

        /// <summary>
        /// Update the specified resource
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="encodedId">The user's base64 encoded user ID</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutData(string url, HttpContent content, string encodedId)
        {
            using (var client = new HttpClient())
            {
                string token = JsonWebTokenServices.CreateJsonWebtoken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(WebServiceConstants.Subscriber, token);
                client.DefaultRequestHeaders.Add("WebRequestUserID", encodedId);
                return await client.PutAsync(new Uri(url), content);
            }
        }

        #region Query search terms

        public static string GeDocumentEventsJsonQuerySearchTerms(string companyId, string userId, string documentId)
        {
            return $"{{\"QuerySearchTerms\":{{\"companyid\":\"{companyId}\",\"userid\":\"{userId}\",\"documentid\":\"{documentId}\"}}}}";
        }

        public static string GetEmailJsonQuerySearchTerms(string email)
        {
            return $"{{\"QuerySearchTerms\":{{\"email\":\"{email}\"}}}}";
        }

        public static string GetCompanyNameJsonQuerySearchTerms(string name)
        {
            return $"{{\"QuerySearchTerms\":{{\"name\":\"{name}\"}}}}";
        }

        public static string GetCompanyIdJsonQuerySearchTerms(string companyid)
        {
            return $"{{\"QuerySearchTerms\":{{\"companyid\":\"{companyid}\"}}}}";
        }

        public static string GetMenuItemsJsonQuerySearchTerms(string email, int parentid)
        {
            return $"{{\"QuerySearchTerms\":{{\"email\":\"{email}\",\"parentid\":\"{parentid}\"}}}}";
        }

        public static string GetJsonQuerySearchTermsToolbarname(string email, string toolbarname)
        {
            return $"{{\"QuerySearchTerms\":{{\"email\":\"{email}\",\"toolbarname\":\"{toolbarname}\"}}}}";
        }

        public static string GetJsonQuerySearchTermsToolbarId(string email, string toolbarid)
        {
            return $"{{\"QuerySearchTerms\":{{\"email\":\"{email}\",\"toolbarid\":\"{toolbarid}\"}}}}";
        }

        public static string GetRoleIdJsonQuerySearchTerms(string roleid)
        {
            return $"{{\"QuerySearchTerms\":{{\"roleid\":\"{roleid}\"}}}}";
        }

        public static string GetJsonQuerySearchTermsCompanyAddresses(int companyId, bool returnall)
        {
            return $"{{\"QuerySearchTerms\":{{\"companyid\":\"{companyId}\",\"returnall\":\"{returnall}\"}}}}";
        }

        public static string GetJsonQuerySearchTermsCompanyAddressId(string companyaddressid)
        {
            return $"{{\"QuerySearchTerms\":{{\"companyaddressid\":\"{companyaddressid}\"}}}}";
        }

        #endregion
    }
}
