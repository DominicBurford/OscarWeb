namespace OscarWeb.Constants
{
    /// <summary>
    /// Defines the constants used by the HttpContext.Session.Set(), HttpContext.Session.Get() and ViewData[]
    /// as the keys used to set and get session / viewdata data.
    /// </summary>
    public class SessionConstants
    {
        //Application related
        public const string WebServicesUrl = "WebServicesUrl";
        public const string KeepSessionAlive = "KeepSessionAlive";

        //Claims related
        public const string UserClaims = "UserClaims";
        public const string EmailClaim = "emails";
        public const string IsNewUserClaim = "newUser";

        //Company related
        public const string CompanyUsers = "CompanyUsers";
        public const string CompanyAddressId = "CompanyAddressId";

        //User related
        public const string CurrentUserId = "CurrentUserId";
        public const string EncodedUserId = "EncodedUserId";
        public const string CurrentUser = "CurrentUser";

        //User-Roles related
        public const string AllUserRoles = "AllUserRoles";
        public const string CurrentlySelectedUserRole = "CurrentlySelectedUserRole";

        //Menu related
        public const string TopLevelMenuItems = "TopLevelMenuItems";
        public const string TopLevelMenuTree = "TopLevelMenuTree";
        public const string DocumentTreeMenu = "DocumentTreeMenu";
        public const string ViewDocumentTreeMenu = "ViewDocumentTreeMenu";
        public const string ViewTopLevelMenuItems = "ViewTopLevelMenuItems";
        public const string DocumentManagerMenuItems = "DocumentManagerMenuItems";
        public const string AdministrationMenuItems = "AdministrationMenuItems";
        public const string CompanyAdministrationMenuItems = "CompanyAdministrationMenuItems";
        public const string CompanyAdministrationParentId = "CompanyAdministrationParentId";
        public const string AdministrationParentId = "AdministrationParentId";
        public const string DocumentManagerParentId = "DocumentManagerParentId";
        public const string KendoMainMenu = "KendoMainMenu";
        public const string ViewKendoMainMenu = "ViewKendoMainMenu";

        //Toolbar related
        public const string DocumentManagerToolbar = "DocumentManagerToolbar";

        //Document related
        public const string DocumentTypes = "DocumentTypes";
        public const string CurrentDocumentId = "CurrentDocumentId";
    }
}
