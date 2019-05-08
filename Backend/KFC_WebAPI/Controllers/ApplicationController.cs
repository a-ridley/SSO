using System;
using System.Collections;
using System.Data.Entity.Validation;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.RequestModels;
using KFC_WebAPI.Models;
using ManagerLayer.ApplicationManagement;
using ServiceLayer.Exceptions;
using ServiceLayer.Services;

namespace KFC_WebAPI.Controllers
{
    public class ApplicationController : ApiController
    {

        private static ApplicationHealthCheck _appHealthStatus = new ApplicationHealthCheck();
        private readonly DatabaseContext _db = new DatabaseContext();

        /// <summary>
        /// Using the application service, obtain a selection of applications based on the 
        /// pagination parameters.
        /// </summary>
        /// <param name="currentPage">The current page the user is viewing</param>
        /// <param name="pageSize">The amount of applications to display</param>
        /// <returns>A response with an object for the total pages and another for the paginated applications</returns>
        [HttpGet, Route("api/applications")]
        public IHttpActionResult GetPaginatedApplications(int currentPage, int pageSize)
        {
            IApplicationService applicationService = new ApplicationService(_db);
            var applications = applicationService.GetPaginatedApplications(currentPage, pageSize, out int totalPages);
            var applicationResponse = new ApplicationResponse(totalPages, applications);
            return Content((HttpStatusCode) 200, applicationResponse);
        }

        /// <summary>
        /// Get all sorted individual applications registered with the SSO
        /// </summary>
        /// <returns>Ok Status Code with the list of all applications registered with the SSO</returns>
        [HttpGet]
        [Route("api/applications/ascending")]
        public IHttpActionResult SortAllApplicationsAscending()
        {
            IApplicationService _applicationService = new ApplicationService(_db);
            var applications = _applicationService.SortAllApplicationsAlphaAscending();
            return Content((HttpStatusCode) 200, applications);
        }

        /// <summary>
        /// Get all sorted individual applications registered with the SSO
        /// </summary>
        /// <returns>Ok Status Code with the list of all applications registered with the SSO</returns>
        [HttpGet]
        [Route("api/applications/descending")]
        public IHttpActionResult SortAllApplicationsDescending()
        {
            IApplicationService _applicationService = new ApplicationService(_db);
            var applications = _applicationService.SortAllApplicationsAlphaDescending();
            return Content((HttpStatusCode)200, applications);
        }

        /// <summary>
        /// Get all sorted individual applications registered with the SSO
        /// </summary>
        /// <returns>Ok Status Code with the list of all applications registered with the SSO</returns>
        [HttpGet]
        [Route("api/applications/clicks")]
        public IHttpActionResult SortAllApplicationsNumOfClicks()
        {
            IApplicationService _applicationService = new ApplicationService(_db);
            var applications = _applicationService.SortAllApplicationsNumOfClicks();
            return Content((HttpStatusCode)200, applications);
        }

        /// <summary>
        /// Register application into portal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/applications/create")]
        public IHttpActionResult Register([FromBody] ApplicationRequest request)
        {
            HttpResponseContent responseContent = new HttpResponseContent(); // Body of Response Message

            if (!ModelState.IsValid || request == null)
            {
                // 412 Response
                responseContent.Code = HttpStatusCode.PreconditionFailed;
                responseContent.Message = "Invalid Request";
                return Content(responseContent.Code, responseContent);
            }

            try
            {
                IApplicationManager manager = new ApplicationManager(_db);
                // Validate request and register application
                responseContent = manager.ValidateRegistration(request);

                responseContent.Code = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                // Request inputs are invalid format or violate business rules
                if (ex is InvalidStringException ||
                    ex is InvalidEmailException ||
                    ex is InvalidUrlException ||
                    ex is ArgumentException)
                {
                    responseContent.Code = HttpStatusCode.BadRequest;
                }
                // Error in data store
                else if (ex is DbEntityValidationException)
                {
                    responseContent.Code = HttpStatusCode.InternalServerError;
                }
                else
                {
                    responseContent.Code = HttpStatusCode.InternalServerError;
                }

                responseContent.Message = ex.Message;
            }
            return Content(responseContent.Code, responseContent);
        }

        /// <summary>
        /// Publish application to portal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/applications/publish")]
        public IHttpActionResult Publish([FromBody] ApplicationRequest request)
        {
            HttpResponseContent responseContent = new HttpResponseContent();  // Body of Response Message

            if (!ModelState.IsValid || request == null)
            {
                // 412 Response
                responseContent.Code = HttpStatusCode.PreconditionFailed;
                responseContent.Message = "Invalid Request";
                return Content(responseContent.Code, responseContent);
            }

            try
            {
                IApplicationManager manager = new ApplicationManager(_db);
                // Validate request inputs and publish application
                responseContent = manager.ValidatePublish(request);

                responseContent.Code = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                // User inputs are invalid format or violate business rules
                if (ex is InvalidStringException ||
                    ex is InvalidUrlException ||
                    ex is InvalidImageException ||
                    ex is InvalidApiKeyException)
                {
                    responseContent.Code = HttpStatusCode.BadRequest;
                }
                // Error in data store
                else if (ex is DbEntityValidationException)
                {
                    responseContent.Code = HttpStatusCode.InternalServerError;
                }
                else
                {
                    responseContent.Code = HttpStatusCode.InternalServerError;
                }

                responseContent.Message = ex.Message;
            }
            return Content(responseContent.Code, responseContent);
        }

        /// <summary>
        /// Generate a new api key for an application
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/applications/generatekey")]
        public IHttpActionResult GenerateKey([FromBody] ApplicationRequest request)
        {
            HttpResponseContent responseContent = new HttpResponseContent();  // Body of Response Message

            if (!ModelState.IsValid || request == null)
            {
                // 412 Response
                responseContent.Code = HttpStatusCode.PreconditionFailed;
                responseContent.Message = "Invalid Request";
                return Content(responseContent.Code, responseContent);
            }

            try
            {

                IApplicationManager manager = new ApplicationManager(_db);
                // Validate request inputs and generate a new api key
                responseContent = manager.ValidateKeyGeneration(request);

                responseContent.Code = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                // User inputs are invalid format or violate business rules
                if (ex is InvalidStringException ||
                    ex is InvalidEmailException ||
                    ex is ArgumentException)
                {
                    responseContent.Code = HttpStatusCode.BadRequest;
                }
                // Error in data store
                else if (ex is DbEntityValidationException)
                {
                    responseContent.Code = HttpStatusCode.InternalServerError;
                }
                else
                {
                    responseContent.Code = HttpStatusCode.InternalServerError;
                }

                responseContent.Message = ex.Message;
            }

            return Content(responseContent.Code, responseContent);
        }

        /// <summary>
        /// Delete application from portal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/applications/delete")]
        public IHttpActionResult DeleteApplication([FromBody] ApplicationRequest request)
        {
            HttpResponseContent responseContent = new HttpResponseContent();  // Body of Response Message

            if (!ModelState.IsValid || request == null)
            {
                // 412 Response
                responseContent.Code = HttpStatusCode.PreconditionFailed;
                responseContent.Message = "Invalid Request";
                return Content(responseContent.Code, responseContent);
            }

            try
            {
                IApplicationManager manager = new ApplicationManager(_db);
                // Validate request inputs and delete application from portal
                responseContent = manager.ValidateDeletion(request);

                responseContent.Code = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                // User inputs are invalid format or violate business rules
                if (ex is InvalidStringException ||
                    ex is InvalidEmailException ||
                    ex is ArgumentException)
                {
                    responseContent.Code = HttpStatusCode.BadRequest;
                }
                // Error in data store
                else if (ex is DbEntityValidationException)
                {
                    responseContent.Code = HttpStatusCode.InternalServerError;
                }
                else
                {
                    responseContent.Code = HttpStatusCode.InternalServerError;
                }

                responseContent.Message = ex.Message;
            }
            return Content(responseContent.Code, responseContent);
        }

        /// <summary>
        /// Update application from portal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/applications/update")]
        public HttpResponseMessage UpdateApplication([FromBody] ApplicationRequest request)
        {
            IApplicationManager manager = new ApplicationManager(_db);
            HttpResponseContent responseContent = manager.ValidateUpdate(request);
            HttpResponseMessage response = Request.CreateResponse(responseContent.Code, responseContent.Message);

            return response;
        }

        /// <summary>
        /// Performs a health check of all applications every minute
        /// Uses a dictionary to store the health status of all applications
        /// The application only gets added to the dictionary if the health
        /// check url return anything other than 200 Success
        /// </summary>
        /// <returns>A timestamp of the last health check and the health statuses of every app</returns>
        [HttpGet]
        [Route("api/applications/healthcheck")]
        public async Task<IHttpActionResult> CheckApplicationHealthCheckAsync()
        {
            // This is checking if the last health check was within the last minute
            if (_appHealthStatus.LastHealthCheck < (DateTime.Now - TimeSpan.FromMinutes(1)))
            {
                // Get the entire list of applications again each time in case they were updated
                // or new ones were added
                ApplicationService _applicationService = new ApplicationService(_db);
                var applications = _applicationService.GetAllApplicationsList();

                // For every app in the applications table, perform a health check
                foreach (Application app in applications)
                {
                    var response = await _applicationService.GetApplicationHealth(app.HealthCheckUrl);
                    // If the status is not a 200 Success, then the the app is down (true)
                    if (!response.IsSuccessStatusCode)
                    {
                        _appHealthStatus.HealthStatuses[app.Id] = true;
                    }
                }
                // Updates last health check to current time
                _appHealthStatus.LastHealthCheck = DateTime.Now;
            }
            // If the the last health check has been one minute then just return the statuses again
            // Or, return the updated statuses if they were updated
            return Content((HttpStatusCode)200, _appHealthStatus);
        }
    }
}