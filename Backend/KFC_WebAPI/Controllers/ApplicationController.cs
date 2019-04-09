using System.Collections;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccessLayer.Database;
using ManagerLayer.ApplicationManagement;
using ServiceLayer.Services;

namespace KFC_WebAPI.Controllers
{
    public class ApplicationController : ApiController
    {
        private ApplicationManager manager = new ApplicationManager();

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
                var applications = ApplicationService.GetAllApplications(_db);
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
                var applications = ApplicationService.SortAllApplicationsAlphaAscending(_db);
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
                var applications = ApplicationService.SortAllApplicationsAlphaDescending(_db);
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
                var applications = ApplicationService.SortAllApplicationsNumOfClicks(_db);
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
        public HttpResponseMessage Register([FromBody] ApplicationRequest request)
        {
            HttpResponseContent responseContent = manager.ValidateRegistration(request);
            HttpResponseMessage response = Request.CreateResponse(responseContent.Code, responseContent);
            
            return  response;
        }

        /// <summary>
        /// Publish application to portal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/applications/publish")]
        public HttpResponseMessage Publish([FromBody] ApplicationRequest request)
        {
            HttpResponseContent responseContent = manager.ValidatePublish(request);
            HttpResponseMessage response = Request.CreateResponse(responseContent.Code, responseContent.Message);

            return response;
        }

        /// <summary>
        /// Generate a new api key
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/applications/generatekey")]
        public HttpResponseMessage GenerateKey([FromBody] ApplicationRequest request)
        {
            HttpResponseContent responseContent = manager.ValidateKeyGeneration(request);
            HttpResponseMessage response = Request.CreateResponse(responseContent.Code, responseContent.Message);

            return response;
        }

        /// <summary>
        /// Delete application from portal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/applications/delete")]
        public HttpResponseMessage DeleteApplication([FromBody] ApplicationRequest request)
        {
            HttpResponseContent responseContent = manager.ValidateDeletion(request);
            HttpResponseMessage response = Request.CreateResponse(responseContent.Code, responseContent.Message);

            return response;
        }

        /// <summary>
        /// Update application from portal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/applications/update")]
        public HttpResponseMessage UpdateApplication([FromBody] ApplicationRequest request)
        {
            HttpResponseContent responseContent = manager.ValidateUpdate(request);
            HttpResponseMessage response = Request.CreateResponse(responseContent.Code, responseContent.Message);

            return response;
        }
    }
}