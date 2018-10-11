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
    public class CompanyAddressController : BaseGridController
    {
        /// <summary>
        /// Populate the grid with data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ActionResult> EditingPopup_Read([DataSourceRequest] DataSourceRequest request)
        {
            //UserModel user = base.GetSession().Get<UserModel>(SessionConstants.CurrentUser);
            string email = base.GetSession().Get<string>(SessionConstants.EmailClaim);
            string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);
            int companyId = base.GetSession().Get<int>(SessionConstants.CompanyAddressId);

            var result = new List<CompanyAddressModel>();

            var addresses =
                await new CompanyAdminPageModelService().GetCompanyAddresses(email, rootUrl, encodedId, companyId, true);
            if (addresses?.Addresses != null && addresses.Addresses.Any())
            {
                result = addresses.Addresses;
            }
            return Json(result.ToDataSourceResult(request));
        }
    }
}
