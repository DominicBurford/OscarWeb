namespace OscarWeb.Constants
{
    public class LoggingServiceConstants
    {
        #region Unit testing constants

        public const string UnitTestEvent = "UnitTestEvent";
        public const string UnitTestTrace = "UnitTestTrace";

        #endregion

        #region User constants

        public const string GetUserDetails = "GetUserDetails";
        public const string GetUserDetailsService = "GetUserDetailsService";
        public const string UpdateUserDetails = "UpdateUserDetails";
        public const string CreateUser = "CreateUser";
        public const string DeleteUser = "DeleteUser";

        #endregion

        #region Menu constants

        public const string GetTopLevelModules = "GetTopLevelModules";
        public const string GetKendoMainMenuItems = "GetKendoMainMenuItems";
        public const string GetMenuItemsModules = "GetMenuItemsModules";
        public const string GetModulesItemsForUser = "GetModulesItemsForUser";
        public const string GetModulesItemsForUserForParentId = "GetModulesItemsForUserForParentId";
        public const string GetModulesItemsForUserWithoutParentId = "GetModulesItemsForUserWithoutParentId";

        #endregion

        #region Toolbar constants

        public const string GetToolbar = "GetToolbar";

        #endregion

        #region Document constants

        public const string GetDocumentTree = "GetDocumentTree";
        public const string GetDocumentForParentUser = "GetDocumentForParentUser";
        public const string GetDocumentById = "GetDocumentById";
        public const string GetDocumentsForDocumentTree = "GetDocumentsForDocumentTree";
        public const string GetDocumentDownload = "GetDocumentDownload";
        public const string GetDocumentTypes = "GetDocumentTypes";
        public const string GetDocumentCategories = "GetDocumentCategories";
        public const string UploadDocumentImage = "UploadDocumentImage";
        public const string UpdateDocument = "UpdateDocument";
        public const string DeleteDocument = "DeleteDocument";
        public const string GetDocumentEvents = "GetDocumentEvents";
        public const string UpdateDocumentTreeParent = "UpdateDocumentTreeParent";
        public const string GetUnreadDocuments = "GetUnreadDocuments";

        #endregion

        #region Company constants

        public const string GetAllCompanies = "GetAllCompanies";
        public const string GetCompanyById = "GetCompanyById";
        public const string GetUsersByCompanyId = "GetUsersByCompanyId";
        public const string CreateCompany = "CreateCompany";
        public const string UpdateCompany = "UpdateCompany";
        public const string DeleteCompany = "DeleteCompany";
        public const string GetCompanyAddresses = "GetCompanyAddresses";
        public const string GetCompanyAddress = "GetCompanyAddress";
        public const string CreateCompanyAddress = "CreateCompanyAddress";
        public const string UpdateCompanyAddress = "UpdateCompanyAddress";
        public const string DeleteCompanyAddress = "DeleteCompanyAddress";

        #endregion

        #region Roles constants

        public const string GetRolesByCompanyId = "GetRolesByCompanyId";
        public const string CreateRole = "CreateRole";
        public const string UpdateRole = "UpdateRole";
        public const string DeleteRole = "DeleteRole";

        #endregion

        #region Email constants

        public const string SendDocumentNotification = "SendDocumentNotification";

        #endregion
    }
}
