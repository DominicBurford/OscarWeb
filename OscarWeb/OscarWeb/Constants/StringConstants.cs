namespace OscarWeb.Constants
{
    public class StringConstants
    {
        #region Document Manager 

        public const string LastViewedNever = "Never";
        public const string DocumentOwner = "You are the owner of this document";
        public const string DocumentSubscriber = "You are a subscriber of this document";
        public const string DocumentFindError = "Could not locate the document. Referesh the page and try again.";
        public const string DocumentDownloadError = "Could not download the document. Check you have the correct permissions. Referesh the page and try again.";
        public const string DocumentDeleteError = "Could not delete the document. Refresh the page and try again.";
        public const string DocumentFileSelectedError = "You can only upload one document.";
        public const string DocumentNoFileSelectedError = "You must select a document to upload.";
        public const string DocumentNoTypeSelected = "Please select a document type.";
        public const string DocumentNoCategorySelected = "Please select a document category.";
        public const string DocumentNoDescription = "Please enter a document description.";
        public const string DocumentUnsupportedFileType = "Document file extension is not supported.";
        public const string DocumentMismatchedFileType = "Document file type does not match file extension.";
        public const string DocumentFolderNoName = "Please enter a folder name";
        public const string DocumentFolderNoDescription = "Please enter a folder description";
        public const string DocumentFolderCreateError = "Error occurred creating folder.";
        public const string DocumentEditError = "Error occurred updating document.";
        public const string DocumentUploadError = "Error occurred uploading document.";
        public const string FolderOwner = "You are the owner of this folder";
        public const string FolderSubscriber = "NA";

        #endregion

        #region Dialog Messages

        public const string DeleteConfirmation = "Are you sure you want to delete this item?";

        #endregion

        #region Main Menu Messages

        public const string NoItemsToDisplay = "No items to display";

        #endregion
    }
}
