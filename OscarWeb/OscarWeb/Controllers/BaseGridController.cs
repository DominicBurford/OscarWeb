using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OscarWeb.Controllers
{
    /// <summary>
    /// Base controller class for all controllers that implement Kendo UI grid functionality
    /// </summary>
    public class BaseGridController : Controller
    {
        /// <summary>
        /// Set this property to unit test the controller
        /// </summary>
        public ISession MockSession { get; set; }

        protected ISession GetSession()
        {
            return this.MockSession ?? HttpContext.Session;
        }
    }
}