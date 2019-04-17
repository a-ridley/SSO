using System;
using System.Collections;
using System.Data.Entity.Validation;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ManagerLayer.ApplicationManagement;
using ServiceLayer.Exceptions;
using ServiceLayer.Services;

namespace KFC_WebAPI.Controllers
{
    public class ApplicationController : ApiController
    {
        //private ApplicationManager manager = new ApplicationManager(_db);

        /// <summary>
        /// Get all individual applications registered with the SSO
        /// </summary>
        /// <returns>Ok Status Code with the list of all applications registered with the SSO</returns>
        [HttpGet]
        [Route("api/applications")]
        public IHttpActionResult GetAllApplications()
        {
            using (var _db = new DatabaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db);
                var applications = _applicationService.GetAllApplications();
                return Content((HttpStatusCode) 200, applications);
            }
        }

        /// <summary>
        /// Get all sorted individual applications registered with the SSO
        /// </summary>
        /// <returns>Ok Status Code with the list of all applications registered with the SSO</returns>
        [HttpGet]
        [Route("api/applications/ascending")]
        public IHttpActionResult SortAllApplicationsAscending()
        {
            using (var _db = new DatabaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db);
                var applications = _applicationService.SortAllApplicationsAlphaAscending();
                return Content((HttpStatusCode)200, applications);
            }
        }

        /// <summary>
        /// Get all sorted individual applications registered with the SSO
        /// </summary>
        /// <returns>Ok Status Code with the list of all applications registered with the SSO</returns>
        [HttpGet]
        [Route("api/applications/descending")]
        public IHttpActionResult SortAllApplicationsDescending()
        {
            using (var _db = new DatabaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db);
                var applications = _applicationService.SortAllApplicationsAlphaDescending();
                return Content((HttpStatusCode)200, applications);
            }
        }

        /// <summary>
        /// Get all sorted individual applications registered with the SSO
        /// </summary>
        /// <returns>Ok Status Code with the list of all applications registered with the SSO</returns>
        [HttpGet]
        [Route("api/applications/clicks")]
        public IHttpActionResult SortAllApplicationsNumOfClicks()
        {
            using (var _db = new DatabaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db);
                var applications = _applicationService.SortAllApplicationsNumOfClicks();
                return Content((HttpStatusCode)200, applications);
            }
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
                using (var _db = new DatabaseContext())
                {
                    IApplicationManager manager = new ApplicationManager(_db);
                    // Validate request and register application
                    responseContent = manager.ValidateRegistration(request);
                }
                    
                responseContent.Code = HttpStatusCode.OK;
            }
            catch(Exception ex)
            {
                // Request inputs are invalid format or violate business rules
                if(ex is InvalidStringException ||
                    ex is InvalidEmailException ||
                    ex is InvalidUrlException ||
                    ex is ArgumentException)
                {
                    responseContent.Code = HttpStatusCode.BadRequest;
                }
                // Error in data store
                else if(ex is DbEntityValidationException)
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
                using (var _db = new DatabaseContext())
                {
                    IApplicationManager manager = new ApplicationManager(_db);
                    // Validate request inputs and publish application
                    responseContent = manager.ValidatePublish(request);
                }

                responseContent.Code = HttpStatusCode.OK;
            }
            catch(Exception ex)
            {
                // User inputs are invalid format or violate business rules
                if(ex is InvalidStringException ||
                    ex is InvalidUrlException ||
                    ex is InvalidImageException ||
                    ex is InvalidApiKeyException)
                {
                    responseContent.Code = HttpStatusCode.BadRequest;
                }
                // Error in data store
                else if(ex is DbEntityValidationException)
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
                using (var _db = new DatabaseContext())
                {
                    IApplicationManager manager = new ApplicationManager(_db);
                    // Validate request inputs and generate a new api key
                    responseContent = manager.ValidateKeyGeneration(request);
                }
                responseContent.Code = HttpStatusCode.OK;
            }
            catch(Exception ex)
            {
                // User inputs are invalid format or violate business rules
                if (ex is InvalidStringException ||
                    ex is InvalidEmailException ||
                    ex is ArgumentException)
                {
                    responseContent.Code = HttpStatusCode.BadRequest;
                }
                // Error in data store
                else if(ex is DbEntityValidationException)
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
                using (var _db = new DatabaseContext())
                {
                    IApplicationManager manager = new ApplicationManager(_db);
                    // Validate request inputs and delete application from portal
                    responseContent = manager.ValidateDeletion(request);
                }

                responseContent.Code = HttpStatusCode.OK;
            }
            catch(Exception ex)
            {
                // User inputs are invalid format or violate business rules
                if (ex is InvalidStringException ||
                    ex is InvalidEmailException ||
                    ex is ArgumentException)
                {
                    responseContent.Code = HttpStatusCode.BadRequest;
                }
                // Error in data store
                else if(ex is DbEntityValidationException)
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
            using (var _db = new DatabaseContext())
            {
                IApplicationManager manager = new ApplicationManager(_db);
                HttpResponseContent responseContent = manager.ValidateUpdate(request);
                HttpResponseMessage response = Request.CreateResponse(responseContent.Code, responseContent.Message);

                return response;
            }
        }
    }
}