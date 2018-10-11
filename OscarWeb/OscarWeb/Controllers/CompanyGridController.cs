using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;

using Common.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace OscarWeb.Controllers
{
    /// <summary>
    /// The controller that is responsible for data operations on the Kendo grid on the Company Admin form.
    ///  </summary>
    public class CompanyGridController : BaseGridController
    {
        /// <summary>
        /// Populate the grid with data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ActionResult> EditingPopup_Read([DataSourceRequest] DataSourceRequest request)
        {
            string email = base.GetSession().Get<string>(SessionConstants.EmailClaim);
            string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);

            var result = new List<CompanyModel>();

            var companies = await new CompanyAdminPageModelService().GetAllCompanies(email, rootUrl, encodedId);
            if (companies != null && companies.Companies.Any())
            {
                result = companies.Companies;
            }
            return Json(result.ToDataSourceResult(request));
        }

        /// <summary>
        /// Add a new company
        /// </summary>
        /// <param name="request"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public async Task<ActionResult> EditingPopup_Create([DataSourceRequest] DataSourceRequest request, CompanyModel company)
        {
            if (!string.IsNullOrEmpty(company?.Name) && ModelState.IsValid)
            {
                string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);

                var sendCompany = this.SupplyDefaultValues(company, true);

                await new CompanyAdminPageModelService().CreateCompany(sendCompany, encodedId, rootUrl);
            }

            return Json(new[] { company }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Update an existing company
        /// </summary>
        /// <param name="request"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public async Task<ActionResult> EditingPopup_Update([DataSourceRequest] DataSourceRequest request, CompanyModel company)
        {
            if (!string.IsNullOrEmpty(company?.Name) && ModelState.IsValid)
            {
                string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);

                var sendCompany = this.SupplyDefaultValues(company, false);

                await new CompanyAdminPageModelService().UpdateCompany(sendCompany, encodedId, rootUrl);
            }

            return Json(new[] { company }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Delete a company
        /// </summary>
        /// <param name="request"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public async Task<ActionResult> EditingPopup_Destroy([DataSourceRequest] DataSourceRequest request, CompanyModel company)
        {
            if (!string.IsNullOrEmpty(company?.Name))
            {
                string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);

                await new CompanyAdminPageModelService().DeleteCompany(company.Name, encodedId, rootUrl);
            }

            return Json(new[] { company }.ToDataSourceResult(request, ModelState));
        }

        private CompanyModel SupplyDefaultValues(CompanyModel company, bool newCompany)
        {
            if (company == null) return null;
            if (string.IsNullOrEmpty(company.StorageContainerName))
            {
                company.StorageContainerName = $"{company.Name.ToLower()}container";
            }
            company.Active = true;
            if (newCompany)
            {
                company.Id = 0;
            }
            return company;
        }
    }
}