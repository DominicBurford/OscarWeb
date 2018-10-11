using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using OscarWeb.Extensions;

using Common.Constants;
using Common.Models;
using OscarWeb.Constants;

namespace OscarWeb.Services
{
    /// <summary>
    /// Class responsible for retrieving the menu options
    /// </summary>
    public class MenuServices
    {
        /// <summary>
        /// Fetch the menu items for the specified user and menu level
        /// </summary>
        /// <param name="useremail"></param>
        /// <param name="parentId"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <returns></returns>
        public async Task<MainMenuModels> GetModulesItemsForUser(string useremail, int parentId, string rooturl, string encodedId)
        {
            MainMenuModels result = null;

            if (string.IsNullOrEmpty(useremail) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) ||
                parentId < 0) return null;

            string queryname = WebTasksTypeConstants.GetMenuItemsForModuleByUser;
            string queryterms = WebApiServices.GetMenuItemsJsonQuerySearchTerms(useremail, parentId);
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response) && response.Length > 0)
                {
                    result = new SerializerServices().DeserializeObject<MainMenuModels>(response.NormalizeJsonString());
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
                    { "ParentId", parentId.ToString() }
                };
                service.TrackEvent(LoggingServiceConstants.GetModulesItemsForUserForParentId, stopwatch.Elapsed, properties);
            }
            return result; 
        }

        /// <summary>
        /// Fetch the menu items for the specified user 
        /// </summary>
        /// <param name="useremail"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <returns></returns>
        public async Task<MainMenuModels> GetModulesItemsForUser(string useremail, string rooturl, string encodedId)
        {
            MainMenuModels result = null;

            if (string.IsNullOrEmpty(useremail) || string.IsNullOrEmpty(rooturl) ||
                string.IsNullOrEmpty(encodedId)) return null;

            string queryname = WebTasksTypeConstants.GetAllMenuItemsForModuleByUser;
            string queryterms = WebApiServices.GetEmailJsonQuerySearchTerms(useremail);
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response) && response.Length > 0)
                {
                    result = new SerializerServices().DeserializeObject<MainMenuModels>(response.NormalizeJsonString());
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
                    { "EncodedId", encodedId }
                };
                service.TrackEvent(LoggingServiceConstants.GetModulesItemsForUserWithoutParentId, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Fetch the parent ID for the specified menu item Display Text
        /// </summary>
        /// <param name="menuitem"></param>
        /// <param name="menuitems"></param>
        /// <returns></returns>
        public int GetParentIdForMenuItem(string menuitem, MainMenuModels menuitems)
        {
            int result = -1;
            if (string.IsNullOrEmpty(menuitem) || menuitems?.MenuItems == null) return result;
            var foundMenuItem = menuitems.MenuItems.Find(m => m.DisplayText == menuitem);
            if (foundMenuItem != null)
            {
                result = foundMenuItem.Id;
            }
            return result;
        }

        /// <summary>
        /// Clean the top level menu items by removing unit test data
        /// </summary>
        /// <param name="menuitems"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public MainMenuModels CleanMainMenuModel_deprecated(MainMenuModels menuitems, UserModel user)
        {
            MainMenuModels displayMenuItems = null;

            if (menuitems?.MenuItems == null || !menuitems.MenuItems.Any() || user == null) return null;
            displayMenuItems = new MainMenuModels();

            foreach (var menuitem in menuitems.MenuItems)
            {
                //filter out the unit testing menu items - only assigned to super users 
                if (user.SuperUser)
                {
                    if (!menuitem.DisplayText.ToLower().Contains("unit test"))
                    {
                        displayMenuItems.MenuItems.Add(menuitem);
                    }
                }
                else
                {
                    displayMenuItems.MenuItems.Add(menuitem);
                }
            }
            return displayMenuItems;
        }

        /// <summary>
        /// Clean the menu items by removing unit test data
        /// </summary>
        /// <param name="menuitems"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public MainMenuModels CleanMainMenuModel(MainMenuModels menuitems, UserModel user)
        {
            MainMenuModels displayMenuItems = null;

            if (menuitems?.MenuItems == null || !menuitems.MenuItems.Any() || user == null) return null;
            displayMenuItems = menuitems;

            if (user.SuperUser)
            {
                foreach (var menuitem in displayMenuItems.MenuItems.ToList())
                {
                    if (menuitem.DisplayText.ToLower().Contains("unit test"))
                    {
                        displayMenuItems.MenuItems.Remove(menuitem);
                        var children = displayMenuItems.MenuItems.FindAll(m => m.ParentId == menuitem.Id);
                        if (children.Any())
                        {
                            displayMenuItems.MenuItems.RemoveAll(m => m.ParentId == menuitem.Id);
                        }
                    }
                }
            }
            return displayMenuItems;
        }
    }
}
