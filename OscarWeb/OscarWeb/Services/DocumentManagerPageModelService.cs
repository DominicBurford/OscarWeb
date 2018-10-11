using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

using OscarWeb.Constants;
using OscarWeb.Extensions;

using Common.Models;
using Common.Constants;
using Common.Models.EmailRequests;

namespace OscarWeb.Services
{
    /// <summary>
    /// Page model service for the Document Manager module.
    /// </summary>
    public class DocumentManagerPageModelService : PageModelService
    {
        public override string ModuleName { get; }

        public DocumentManagerPageModelService()
        {
            ModuleName = ModuleNameConstants.DocumentManager;
        }
        
        /// <summary>
        /// Return the documents for the specified user and document level
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<DocumentsModels> GetDocumentForParentUser(string email, string rooturl, string encodedId, int parentId)
        {
            DocumentsModels result = null;
            string url = $"{rooturl}api/documents?parentId={parentId}&useremail={email}";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(encodedId) ||
                parentId < 0) return null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<DocumentsModels>(response.NormalizeJsonString());
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
                    { "ParentId", parentId.ToString() }
                };
                service.TrackEvent(LoggingServiceConstants.GetDocumentForParentUser, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Returns a specified document by its ID
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<DocumentsModel> GetDocumentById(string email, string rooturl, string encodedId, int documentId)
        {
            DocumentsModel result = null;
            string url = $"{rooturl}api/documents?documentId={documentId}";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(encodedId) ||
                documentId <= 0) return null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string response = await new WebApiServices().GetData(url, encodedId);

                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<DocumentsModel>(response.NormalizeJsonString());
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
                    { "DocumentId", documentId.ToString() }
                };
                service.TrackEvent(LoggingServiceConstants.GetDocumentById, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Returns a specified document by its model properties.
        /// The following model properties must be supplied:
        /// model.CompanyId
        /// model.UploadedBy
        /// model.Name
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<DocumentsModel> GetDocumentByModel(string email, string rooturl, string encodedId, DocumentsModel document)
        {
            DocumentsModel result = null;
            
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) ||
                document == null || document.CompanyId <= 0 || string.IsNullOrEmpty(document.Name) || document.UploadedBy <= 0) return null;

            string payload = new SerializerServices().SerializeObject(document);
            string url = $"{rooturl}api/documents?postdata={payload}";

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string response = await new WebApiServices().GetData(url, encodedId);

                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<DocumentsModel>(response.NormalizeJsonString());
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
                    { "DocumentName", document.Name }
                };
                service.TrackEvent(LoggingServiceConstants.GetDocumentById, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Return the documents for the specified user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <returns></returns>
        public async Task<DocumentTreeModels> GetDocumentsForDocumentTree(string email, string rooturl, string encodedId)
        {
            DocumentTreeModels result = null;
            string url = $"{rooturl}api/documents?useremail={email}";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(encodedId)) return null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                string response = await new WebApiServices().GetData(url, encodedId);

                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<DocumentTreeModels>(response.NormalizeJsonString());
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
                service.TrackEvent(LoggingServiceConstants.GetDocumentsForDocumentTree, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Return the toolbar items for the specified user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="toolbarname"></param>
        /// <returns></returns>
        public async Task<ToolbarModel> GetToolbarForUser(string email, string rooturl, string encodedId, string toolbarname)
        {
            return await new ToolbarServices().GetToolbar(email, rooturl, encodedId, toolbarname);
        }

        /// <summary>
        /// Returns a decorated instance of a <see cref="DocumentsModel"/> object for display
        /// </summary>
        /// <param name="response"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ExpandoObject GetDocumentForDisplay(DocumentsModel response, int userId)
        {
            //use an ExpandoObject to decorate the return object with dynamic properties
            dynamic document = new ExpandoObject();
            document.Description = response.Description;
            if (response.LastViewed == DateTime.MinValue)
            {
                document.LastView = StringConstants.LastViewedNever;
            }
            else
            {
                document.LastView = response.LastViewed;
            }
            document.DocumentFolder = response.IsDocument ? "Document" : "Folder";
            document.Created = response.Created;
            document.LastViewed = response.LastViewed;
            document.UploadedByUsername = response.UploadedByUsername;
            document.DocumentTypeDescription = response.DocumentTypeDescription;
            document.DocumentCategoryDescription = response.DocumentCategoryDescription;
            document.OwnerSubscriber = userId == response.UploadedBy ? StringConstants.DocumentOwner : StringConstants.DocumentSubscriber;
            if (response.IsDocument)
            {
                document.OwnerSubscriber = userId == response.UploadedBy ? StringConstants.DocumentOwner : StringConstants.DocumentSubscriber;
            }
            else
            {
                document.OwnerSubscriber = userId == response.UploadedBy ? StringConstants.FolderOwner : StringConstants.FolderSubscriber;
            }
            if (response.Subscribers != null && response.Subscribers.Any())
            {
                document.Subscribers = new List<string>();
                foreach (var subscriber in response.Subscribers)
                {
                    document.Subscribers.Add(subscriber.Username);
                }
            }
            return document;
        }

        /// <summary>
        /// Retrieve the document as a data stream
        /// </summary>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<DocumentDownloadModel> GetDocumentDownload(string rooturl, string encodedId, int documentId)
        {
            if (documentId <= 0 || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId)) return null;
            string url = $"{rooturl}api/documentimages?documentId={documentId}";
            DocumentDownloadModel result = null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<DocumentDownloadModel>(response.NormalizeJsonString());
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
                    { "WebServicesEndpoint", rooturl },
                    { "EncodedId", encodedId },
                    { "DocumentId", documentId.ToString() }
                };
                service.TrackEvent(LoggingServiceConstants.GetDocumentDownload, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Retrieves a list of document types
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <returns></returns>
        public async Task<DocumentTypeModels> GetDocumentTypes(string email, string rooturl, string encodedId)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId)) return null;

            string queryname = WebTasksTypeConstants.GetDocumentTypes;
            string queryterms = WebApiServices.GetEmailJsonQuerySearchTerms(email);
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";
            DocumentTypeModels result = null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<DocumentTypeModels>(response.NormalizeJsonString());
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
                service.TrackEvent(LoggingServiceConstants.GetDocumentTypes, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Retrieves a list of document categories
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <returns></returns>
        public async Task<DocumentCategoryModels> GetDocumentCategories(string email, string rooturl, string encodedId)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId)) return null;

            string queryname = WebTasksTypeConstants.GetDocumentCategories;
            string queryterms = WebApiServices.GetEmailJsonQuerySearchTerms(email);
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";
            DocumentCategoryModels result = null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<DocumentCategoryModels>(response.NormalizeJsonString());
                    result = this.CleanDocumentCategories(result);
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
                service.TrackEvent(LoggingServiceConstants.GetDocumentCategories, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Upload a document and associated image
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="documentImage"></param>
        /// <returns></returns>
        public async Task<bool> UploadDocumentImage(string email, string rooturl, string encodedId,
            DocumentImageModel documentImage)
        {
            bool result = false;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) ||
                documentImage?.Document == null || documentImage.Image == null) return false;

            string payload = new SerializerServices().SerializeObject(documentImage);
            string url = $"{rooturl}api/documentimages";
            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
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
                    { "UserEmail", email },
                    { "WebServicesEndpoint", rooturl },
                    { "EncodedId", encodedId },
                    { "DocumentName", documentImage.Document.Name }
                };
                service.TrackEvent(LoggingServiceConstants.UploadDocumentImage, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Update a document
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<bool> UpdateDocument(string email, string rooturl, string encodedId,
            DocumentsModel document)
        {
            bool result = false;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) ||
                document?.Name == null || document.CompanyId <= 0 || document.UploadedBy <= 0) return false;

            string payload = new SerializerServices().SerializeObject(document);
            string url = $"{rooturl}api/documents?action={DocumentActionTypeConstants.DocumentActionUpsert}";
            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
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
                    { "UserEmail", email },
                    { "WebServicesEndpoint", rooturl },
                    { "EncodedId", encodedId },
                    { "DocumentId", document.Id.ToString() }
                };
                service.TrackEvent(LoggingServiceConstants.UpdateDocument, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Delete a document
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteDocument(string email, string rooturl, string encodedId, int documentId)
        {
            bool result = false;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) ||
                documentId <= 0) return false;

            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.DeleteDocument}&webDeleteId={documentId}";

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
                    { "UserEmail", email },
                    { "WebServicesEndpoint", rooturl },
                    { "EncodedId", encodedId },
                    { "DocumentId", documentId.ToString() }
                };
                service.TrackEvent(LoggingServiceConstants.DeleteDocument, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Return the Document Type for the specified Document Type ID
        /// </summary>
        /// <param name="documentTypeId"></param>
        /// <param name="documentTypes"></param>
        /// <returns></returns>
        public DocumentTypeModel GetDocumentTypeForDocument(int documentTypeId, DocumentTypeModels documentTypes)
        {
            DocumentTypeModel result = null;
            if (documentTypes?.DocumentTypes == null || !documentTypes.DocumentTypes.Any() || documentTypeId <= 0)
                return null;
            result = new DocumentTypeModel();
            foreach (var documentType in documentTypes.DocumentTypes)
            {
                if (documentType.Id != documentTypeId) continue;
                result = documentType;
                break;
            }
            return result;
        }

        /// <summary>
        /// Returns a list of document subscribers from the subscriber 
        /// control on the document upload form DocumentUpload.cshtml
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        public List<DocumentSubscriberModel> ToDocumentSubscribers(StringValues strings, UserModels users)
        {
            if (strings.Count <= 0 || users?.Users == null || !users.Users.Any()) return null;
            return strings.Select(subscriber => new DocumentSubscriberModel
                {
                    Active = true,
                    LastViewed = DateTime.MinValue,
                    UserId = this.GetUserIdFromUserList(subscriber, users)
                })
                .ToList();
        }

        /// <summary>
        /// Returns a specific user from a list of users by their username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        public int GetUserIdFromUserList(string username, UserModels users)
        {
            if (string.IsNullOrEmpty(username) || users?.Users == null || !users.Users.Any()) return 0;
            return users.Users.Find(u => u.UserName == username).Id;
        }

        /// <summary>
        /// Determines of the file extension is supported
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="documentTypes"></param>
        /// <returns></returns>
        public bool IsFileTypeSupported(string fileExtension, DocumentTypeModels documentTypes)
        {
            if (string.IsNullOrEmpty(fileExtension) || documentTypes?.DocumentTypes == null || !documentTypes.DocumentTypes.Any()) return false;

            bool result = false;
            foreach (var documentType in documentTypes.DocumentTypes)
            {
                if (string.Compare(fileExtension, documentType.Extension,
                        StringComparison.OrdinalIgnoreCase) != 0) continue;
                result = true;
                break;
            }
            return result;
        }

        /// <summary>
        /// Send the notifications to the document subscribers
        /// </summary>
        /// <param name="document"></param>
        /// <param name="rooturl"></param>
        /// <returns></returns>
        public async Task<bool> NotifySubscribers(DocumentsModel document, string rooturl)
        {
            if (document?.Subscribers == null || !document.Subscribers.Any() || string.IsNullOrEmpty(document.Name) || string.IsNullOrEmpty(document.UploadedByUsername)) return false;
            bool result = true;
            foreach (var subscriber in document.Subscribers)
            {
                var notification =
                    new DocumentSubscriberNotificationModel
                    {
                        UserName = subscriber.Username,
                        DocumentName = document.Name,
                        UploaderName = document.UploadedByUsername,
                        EmailRecipient = new List<string>()
                    };
                notification.EmailRecipient.Add(subscriber.Email);
                var response = await new SendEmailService().SendDocumentNotification(notification, rooturl);
                result = result && response;
            }
            return result;
        }

        /// <summary>
        /// Update the document subscriber email and user names
        /// </summary>
        /// <param name="document"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        public DocumentsModel UpdateDocumentSubscribers(DocumentsModel document, UserModels users)
        {
            if (document?.Subscribers == null || !document.Subscribers.Any() || users?.Users == null || !users.Users.Any()) return document;
            foreach (var t in document.Subscribers)
            {
                var user = users.Users.Find(u => u.Id == t.UserId);
                t.Username = user.UserName;
                t.Email = user.Email;
            }
            return document;
        }

        /// <summary>
        /// Retrieves a list of document events for the specified company, user or document.
        /// Pass a null as an argument where the argument should be ignored.
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<DocumentEventModels> GetDocumentEvents(string companyId, string userId, string documentId, string rooturl, string encodedId)
        {
            if (string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId)) return null;

            string queryname = WebTasksTypeConstants.GetDocumentEvents;
            string queryterms = WebApiServices.GeDocumentEventsJsonQuerySearchTerms(companyId, userId, documentId);
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";
            DocumentEventModels result = null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<DocumentEventModels>(response.NormalizeJsonString());
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
                    { "CompanyId", companyId },
                    { "UserId", userId },
                    { "DocumentId", documentId },
                    { "WebServicesEndpoint", rooturl },
                    { "EncodedId", encodedId }
                };
                service.TrackEvent(LoggingServiceConstants.GetDocumentEvents, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Update the parent for a specified document
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <param name="destinationId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateDocumentTreeParent(int destinationId, int sourceId, string rooturl, string encodedId)
        {
            if (string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId) || destinationId < 0 || sourceId < 0) return false;
            
            string url = $"{rooturl}api/webtasks?formname={RoutingTasksTypeConstants.DocumentDragDrop}&useRoutingController=true";
            DocumentDragDropModel model = new DocumentDragDropModel
            {
                DestinationId = destinationId,
                SourceId = sourceId
            };
            string payload = new SerializerServices().SerializeObject(model);
            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
            bool result = false;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
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
                    { "DestinationId", destinationId.ToString() },
                    { "SourceId", sourceId.ToString() },
                    { "WebServicesEndpoint", rooturl },
                    { "EncodedId", encodedId }
                };
                service.TrackEvent(LoggingServiceConstants.UpdateDocumentTreeParent, stopwatch.Elapsed, properties);
            }
            return result;
        }

        /// <summary>
        /// Fetch all unread documents for the specified user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="rooturl"></param>
        /// <param name="encodedId"></param>
        /// <returns></returns>
        public async Task<DocumentUnreadModels> GetUnreadDocuments(string email, string rooturl, string encodedId)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rooturl) || string.IsNullOrEmpty(encodedId)) return null;

            string queryname = WebTasksTypeConstants.GetUnreadDocuments;
            string queryterms = WebApiServices.GetEmailJsonQuerySearchTerms(email);
            string url = $"{rooturl}api/webtasks?queryname={queryname}&queryterms={queryterms}";
            DocumentUnreadModels result = null;

            LoggingService service = new LoggingService();
            var stopwatch = new Stopwatch();

            try
            {
                var response = await new WebApiServices().GetData(url, encodedId);
                if (!string.IsNullOrEmpty(response))
                {
                    result = new SerializerServices()
                        .DeserializeObject<DocumentUnreadModels>(response.NormalizeJsonString());
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
                service.TrackEvent(LoggingServiceConstants.GetUnreadDocuments, stopwatch.Elapsed, properties);
            }
            return result;
        }

        private DocumentCategoryModels CleanDocumentCategories(DocumentCategoryModels categories)
        {
            DocumentCategoryModels result = null;
            if (categories?.DocumentCategories == null || !categories.DocumentCategories.Any()) return null;
            result = new DocumentCategoryModels();
            foreach (var category in categories.DocumentCategories)
            {
                if (category.Description.ToLower() != "unit testing")
                {
                    result.DocumentCategories.Add(category);
                }
            }
            return result;
        }
    }
}
