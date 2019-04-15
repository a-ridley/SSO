using DataAccessLayer.Database;
using DataAccessLayer.Models;
using MimeKit;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;

namespace ManagerLayer.ApplicationManagement
{
    public class ApplicationManager
    {
        public ApplicationManager()
        {
            // TODO: set up email server to implement email services
            _emailService = new EmailService();
            _tokenService = new TokenService();
        }

        // Business Rules
        private const int titleLength = 100;
        private const int descriptionLength = 2000;
        private const int xDimension = 55;
        private const int yDimension = 55;
        private const string imageType = ".PNG";
        private const int urlLength = 500;

        // Services
        private IEmailService _emailService;
        private ITokenService _tokenService;

        /// <summary>
        /// Validate the app registration field values, and call registration services
        /// </summary>
        /// <param name="request">Values from POST request</param>
        /// <returns>Http status code and message</returns>
        public HttpResponseContent ValidateRegistration(ApplicationRequest request)
        {
            // Http status code and message
            HttpResponseContent response;

            if (request == null)
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Request.");
                return response;
            }

            Uri launchUrl = null;
            Uri deleteUrl = null;

            // Validate request values
            if (request.Title == null || !IsValidStringLength(request.Title, titleLength))
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Title: Cannot be more than 100 characters in length.");
                return response;
            }
            else if (request.Email == null || !IsValidEmail(request.Email))
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Email");
                return response;
            }
            else if (request.LaunchUrl == null || !IsValidUrl(request.LaunchUrl, ref launchUrl) || !IsValidStringLength(request.LaunchUrl, urlLength))
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Application Url");
                return response;
            }
            else if (request.DeleteUrl == null || !IsValidUrl(request.DeleteUrl, ref deleteUrl) || !IsValidStringLength(request.DeleteUrl, urlLength))
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid User Deletion Url");
                return response;
            }

            // Create a new application
            Application app = new Application
            {
                Title = request.Title,
                LaunchUrl = launchUrl.ToString(),
                Email = request.Email,
                UserDeletionUrl = request.DeleteUrl,
                SharedSecretKey = _tokenService.GenerateToken()
            };

            // Create a new ApiKey
            ApiKey apiKey = new ApiKey
            {
                // Generate a unique key
                Key = _tokenService.GenerateToken(),
                ApplicationId = app.Id
            };

            using (var _db = new DatabaseContext())
            {
                // Attempt to create an Application record
                var appResponse = ApplicationService.CreateApplication(_db, app);
                if (appResponse == null)
                {
                    response = new HttpResponseContent(HttpStatusCode.BadRequest, "Application Already Exists");
                    return response;
                }

                // Attempt to create an ApiKey record
                var keyResponse = ApiKeyService.CreateKey(_db, apiKey);
                // Keep generating a new key until a unique one is made.
                while (keyResponse == null)
                {
                    apiKey.Key = _tokenService.GenerateToken();
                    keyResponse = ApiKeyService.CreateKey(_db, apiKey);
                }

                List<object> responses = new List<object>();
                responses.Add(appResponse);
                responses.Add(keyResponse);

                // Save database changes
                if (!SaveChanges(_db, responses))
                {
                    response = new HttpResponseContent(HttpStatusCode.InternalServerError, "Unable to save database changes");
                    return response;
                }
            }

            string message;

            // Email the application ID, api key, and shared secret key
            try
            {
                SendAppRegistrationEmail(app.Email, apiKey.Key, app.SharedSecretKey, app.Id);
                message = "Successful Registration!  An email has also been sent containing the following information.";
            }
            catch
            {
                message = "Successful Registration!  Please save the following information.  An email containing this information was unable to send.";
            }

            // Return success response
            response = new HttpResponseContent(HttpStatusCode.OK, message, apiKey.Key, app.SharedSecretKey, app.Id);
            return response;
        }

        /// <summary>
        /// Validate the app publish field values, and call publish services
        /// </summary>
        /// <param name="request">Values from POST request</param>
        /// <returns>Http status code and message</returns>
        public HttpResponseContent ValidatePublish(ApplicationRequest request)
        {
            // Http status code and message
            HttpResponseContent response;

            if (request == null)
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Request.");
                return response;
            }

            Uri logoUrl = null;

            // Validate publish request values
            if (request.Title == null)
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Title.");
                return response;
            }
            else if (!IsValidStringLength(request.Description, descriptionLength))
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Description: Cannot be more than 2000 characters in length.");
                return response;
            }
            else if (!IsValidUrl(request.LogoUrl, ref logoUrl))
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Logo Url.");
                return response;
            }
            else if (!IsValidImageExtension(logoUrl, imageType))
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Logo Image Extension: Can only be .PNG");
                return response;
            }
            else if (!IsValidDimensions(logoUrl,xDimension,yDimension))
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Logo Dimensions: Can be no more than 55x55 pixels.");
                return response;
            }

            using (var _db = new DatabaseContext())
            {
                // Attempt to find api key
                var apiKey = ApiKeyService.GetKey(_db, request.Key);

                // Key must exist and be unused.
                if (apiKey == null || apiKey.IsUsed == true)
                {
                    response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Key");
                    return response;
                }

                // Attempt to get application based on ApplicationId from api key
                var app = ApplicationService.GetApplication(_db, apiKey.ApplicationId);

                // Published application title is used to authenticate the app.
                if (app == null || !request.Title.Equals(app.Title))
                {
                    response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Key");
                    return response;
                }

                // Update values of application record
                app.Description = request.Description;
                app.LogoUrl = request.LogoUrl;
                app.UnderMaintenance = request.UnderMaintenance;
                var appResponse = ApplicationService.UpdateApplication(_db, app);

                // Update values of api key record
                apiKey.IsUsed = true;
                var keyResponse = ApiKeyService.UpdateKey(_db, apiKey);

                List<object> responses = new List<object>();
                responses.Add(appResponse);
                responses.Add(keyResponse);

                // Attempt to save database changes
                if (!SaveChanges(_db, responses))
                {
                    // Error response
                    response = new HttpResponseContent(HttpStatusCode.InternalServerError, "Unable to save database changes");
                    return response;
                }

                string message;

                // Email successful publish confirmation
                try
                {
                    SendAppPublishEmail(app.Email, app);
                    message = "Successful Publish to the KFC SSO portal!  An email has been sent confirming your publish.";
                }
                catch
                {
                    message = "Successful Publish to the KFC SSO portal!  A confirmation email was unable to send.";
                }

                // Return successful publish response
                response = new HttpResponseContent(HttpStatusCode.OK, message);
                return response;
            }

        }

        /// <summary>
        /// Validate the key generation field values, and call key generation services
        /// </summary>
        /// <param name="request">Values from POST request</param>
        /// <returns>Http status code and message</returns>
        public HttpResponseContent ValidateKeyGeneration(ApplicationRequest request)
        {
            // Http status code and message
            HttpResponseContent response;

            if (request == null)
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Request.");
                return response;
            }

            // Validate key generation request values
            if (request.Title == null)
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Title");
                return response;
            }
            else if (request.Email == null || !IsValidEmail(request.Email))
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Email");
                return response;
            }

            using (var _db = new DatabaseContext())
            {
                // Attempt to find application
                var app = ApplicationService.GetApplication(_db, request.Title, request.Email);
                if (app == null)
                {
                    response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Application");
                    return response;
                }

                // Create a new ApiKey
                ApiKey apiKey = new ApiKey
                {
                    Key = _tokenService.GenerateToken(),
                    ApplicationId = app.Id
                };

                // Invalidate old unused api key
                var keyOld = ApiKeyService.GetKey(_db, app.Id, false);
                if(keyOld != null)
                {
                    keyOld.IsUsed = true;
                    keyOld = ApiKeyService.UpdateKey(_db, keyOld);
                }

                // Attempt to create an apiKey record
                var keyResponse = ApiKeyService.CreateKey(_db, apiKey);

                // Keep generating a new key until a unique one is made.
                while (keyResponse == null)
                {
                    apiKey.Key = _tokenService.GenerateToken();
                    keyResponse = ApiKeyService.CreateKey(_db, apiKey);
                }

                List<object> responses = new List<object>();
                responses.Add(keyResponse);
                responses.Add(keyOld);

                // Save database changes
                if (!SaveChanges(_db, responses))
                {
                    response = new HttpResponseContent(HttpStatusCode.InternalServerError, "Unable to save database changes");
                    return response;
                }

                string message;

                // Email the new api key
                try
                {
                    SendNewApiKeyEmail(app.Email, apiKey.Key);
                    message = "Successful Key Generation!  An email has been sent containing your new API Key.";
                }
                catch
                {
                    message = "Successful Key Generation!  Please save the following information.  An email containing this information was unable to send.";
                }

                // Return successful key generation response
                response = new HttpResponseContent(HttpStatusCode.OK, message, apiKey.Key);
                return response;
            }

        }

        /// <summary>
        /// Validate the App Deletion field values, and call deletion services
        /// </summary>
        /// <param name="request">Values from POST request</param>
        /// <returns>Http status code and message</returns>
        public HttpResponseContent ValidateDeletion(ApplicationRequest request)
        {
            // Http status code and message
            HttpResponseContent response;

            // Validate deletion request values
            if (request.Title == null)
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Title");
                return response;
            }
            else if (!IsValidEmail(request.Email))
            {
                response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Email");
                return response;
            }

            using (var _db = new DatabaseContext())
            {
                // Attempt to find application
                var app = ApplicationService.GetApplication(_db, request.Title, request.Email);
                if (app == null)
                {
                    response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Application");
                    return response;
                }

                // Attempt to create an apiKey record
                var appResponse = ApplicationService.DeleteApplication(_db, app.Id);
                if (appResponse == null)
                {
                    response = new HttpResponseContent(HttpStatusCode.InternalServerError, "Unable to delete application.");
                    return response;
                }

                List<object> responses = new List<object>();
                responses.Add(appResponse);

                // Save database changes
                if (!SaveChanges(_db, responses))
                {
                    response = new HttpResponseContent(HttpStatusCode.InternalServerError, "Unable to save database changes");
                    return response;
                }

                string message;

                // Email application deletion confirmation
                try
                {
                    SendAppDeleteEmail(app.Email, app.Title);
                    message = "Application Deleted.  An email has been sent confirming your deletion.";
                }
                catch
                {
                    message = "Application Deleted.  A confirmation email was unable to send.";
                }

                // Return successful deletion response
                response = new HttpResponseContent(HttpStatusCode.OK, message);
                return response;
            }
        }


        public HttpResponseContent ValidateUpdate(ApplicationRequest request)
        {
            // Http status code and message
            HttpResponseContent response;

            using (var _db = new DatabaseContext())
            {
                // Attempt to find application
                var app = ApplicationService.GetApplication(_db, request.Title, request.Email);
                if (app == null)
                {
                    response = new HttpResponseContent(HttpStatusCode.BadRequest, "Invalid Application");
                    return response;
                }

                // Update click count of application record

                app.ClickCount = request.ClickCount;
                var appResponse = ApplicationService.UpdateApplication(_db, app);

                List<object> responses = new List<object>();
                responses.Add(appResponse);

                // Attempt to save database changes
                if (!SaveChanges(_db, responses))
                {
                    // Error response
                    response = new HttpResponseContent(HttpStatusCode.InternalServerError, "Unable to save database changes");
                    return response;
                }

                // Successful publish
                response = new HttpResponseContent(HttpStatusCode.OK, "Updated application from SSO");
                return response;
            }
        }

        /// <summary>
        /// Validates a string length
        /// </summary>
        /// <param name="length">string</param>
        /// <returns></returns>
        public bool IsValidStringLength(string s, int length)
        {
            if (s == null || s.Length > length)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates email format
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsValidEmail(string email)
        {
            try
            {
                // Check for a valid email.
                var valid = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                // Invalid email
                return false;
            }
        }

        /// <summary>
        /// Validates url format
        /// </summary>
        /// <param name="url"></param>
        /// <param name="uriResult"></param>
        /// <returns></returns>
        public bool IsValidUrl(string url, ref Uri uriResult)
        {
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

        /// <summary>
        /// Validates image file type
        /// </summary>
        /// <param name="imageUrl">image url</param>
        /// <param name="ex">file type</param>
        /// <returns></returns>
        public bool IsValidImageExtension(Uri imageUrl, string type)
        {
            if(imageUrl == null)
            {
                return false;
            }
            string extension = Path.GetExtension("@" + imageUrl.ToString());

            // Logo can only be of .PNG image file type.
            if (!extension.ToUpper().Equals(type))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates image dimensions
        /// </summary>
        /// <param name="imgUrl">image url</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool IsValidDimensions(Uri imgUrl, int width, int height)
        {
            if(imgUrl == null)
            {
                return false;
            }

            // Download image
            WebClient wc = new WebClient();
            byte[] bytes = wc.DownloadData(imgUrl);
            MemoryStream ms = new MemoryStream(bytes);
            Image img = Image.FromStream(ms);

            // Get dimensions
            int x, y;
            x = img.Width;
            y = img.Height;
            
            if(x > width || y > height)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Save the changes made to the database tables
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="responses">changes made</param>
        /// <returns>Whether the changes were saved</returns>
        public bool SaveChanges(DatabaseContext _db, List<object> responses)
        {
            try
            {
                // Save changes in the database
                _db.SaveChanges();

                return true;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException)
            {
                // Catch error
                // Detach item attempted to be changed from the db context - rollback
                foreach(object response in responses)
                {
                    _db.Entry(response).State = System.Data.Entity.EntityState.Detached;
                }

                // Error
                return false;
            }
        }

        #region Emails

        /// <summary>
        /// Creates an email to send the application id, api key,
        /// and shared secret key to newly registered application.
        /// </summary>
        /// <param name="receiverEmail"></param>
        /// <param name="apiKey"></param>
        /// <param name="sharedSecretKey"></param>
        /// <param name="appId"></param>
        public void SendAppRegistrationEmail(string receiverEmail, string apiKey, string sharedSecretKey, Guid appId)
        {
            string appRegisterSubjectString = "KFC SSO: Application Registration";
            string userFullName = receiverEmail;
            string template = "Hi, \r\n \r\n" +
                                             "You recently registered your application to the KFC SSO portal.\r\n \r\n" +
                                             "Your Application ID:\r\n {0} \r\n \r\n" +
                                             "A single-use API Key to publish your application into the portal:\r\n {1} \r\n \r\n" +
                                             "The Shared Secret Key between your application and the KFC SSO Portal.\r\n {2} \r\n \r\n" +
                                             "If you did not register your application to KFC, please contact us by responding to this email.\r\n\r\n" +
                                             "Thanks, \r\nKFC Team";

            string appRegisterBodyString = string.Format(template, appId, apiKey, sharedSecretKey);

            //Create the message that will be sent
            MimeMessage emailToSend = _emailService.CreateEmailPlainBody(userFullName, receiverEmail, appRegisterSubjectString, appRegisterBodyString);
            //Send the email with the message
            _emailService.SendEmail(emailToSend);
        }

        /// <summary>
        /// Creates an email to send a confirmation of app publish.
        /// </summary>
        /// <param name="receiverEmail"></param>
        /// <param name="app"></param>
        public void SendAppPublishEmail(string receiverEmail, Application app)
        {
            string appPublishSubjectString = "KFC SSO: Application Publish";
            string userFullName = receiverEmail;
            string template = "Hi, \r\n \r\n" +
                                             "Your application, {0}, was successfully published to the KFC SSO portal with the following details:\r\n \r\n" +
                                             "Description: {1}\r\n" +
                                             "Logo URL: {2}\r\n" +
                                             "Under Maintenance: {3}\r\n \r\n" +
                                             "If you did not make this request, please contact us by responding to this email.\r\n \r\n" +
                                             "Thanks, \r\nKFC Team";
            
            string appPublishBodyString = string.Format(template, app.Title, app.Description, app.LogoUrl, app.UnderMaintenance);

            //Create the message that will be sent
            MimeMessage emailToSend = _emailService.CreateEmailPlainBody(userFullName, receiverEmail, appPublishSubjectString, appPublishBodyString);
            //Send the email with the message
            _emailService.SendEmail(emailToSend);
        }

        /// <summary>
        /// Creates an email to send new api keys to applications.
        /// </summary>
        /// <param name="receiverEmail"></param>
        /// <param name="apiKey"></param>
        public void SendNewApiKeyEmail(string receiverEmail, string apiKey)
        {
            string generateKeySubjectString = "KFC SSO: New API Key";
            string userFullName = receiverEmail;
            string template = "Hi, \r\n \r\n" +
                                             "You recently requested a new API Key for you KFC application.\r\n \r\n" +
                                             "Below is a new single-use API Key to publish your application into the portal.\r\n {0} \r\n \r\n" +
                                             "If you did not make this request, please contact us by responding to this email.\r\n \r\n" +
                                             "Thanks, \r\nKFC Team";
            
            string generateKeyBodyString = string.Format(template, apiKey);

            //Create the message that will be sent
            MimeMessage emailToSend = _emailService.CreateEmailPlainBody(userFullName, receiverEmail, generateKeySubjectString, generateKeyBodyString);
            //Send the email with the message
            _emailService.SendEmail(emailToSend);
        }

        /// <summary>
        /// Creates an email to send a confirmation of app deletion.
        /// </summary>
        /// <param name="receiverEmail"></param>
        /// <param name="appTitle"></param>
        public void SendAppDeleteEmail(string receiverEmail, string appTitle)
        {
            string appDeleteSubjectString = "KFC SSO: Application Deletion";
            string userFullName = receiverEmail;
            string template = "Hi, \r\n \r\n" +
                                             "Your application, {0}, was successfully deleted from the KFC SSO portal.\r\n \r\n" +
                                             "If you did not make this request, please contact us by responding to this email.\r\n \r\n" +
                                             "Thanks, \r\nKFC Team";

            string appDeleteBodyString = string.Format(template, appTitle);

            //Create the message that will be sent
            MimeMessage emailToSend = _emailService.CreateEmailPlainBody(userFullName, receiverEmail, appDeleteSubjectString, appDeleteBodyString);
            //Send the email with the message
            _emailService.SendEmail(emailToSend);
        }

        #endregion
    }
}
