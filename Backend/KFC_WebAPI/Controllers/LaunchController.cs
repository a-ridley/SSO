using DataAccessLayer.Database;
using DataAccessLayer.Models;
using KFC_WebAPI.RequestModels;
using ManagerLayer;
using ManagerLayer.LaunchManagement;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Web.Http;

namespace KFC_WebAPI.Controllers
{
    public class LaunchController : ApiController
    {
        [HttpGet]
        [Route("api/launch")]
        public IHttpActionResult Launch([FromUri, Required]LaunchRequest request)
        {
            if (!ModelState.IsValid || request == null)
            {
                // Request does not match model
                return Content(System.Net.HttpStatusCode.PreconditionFailed, ModelState);
            }

            using (var _db = new DatabaseContext())
            {
                // Validate session
                IAuthorizationManager authorizationManager = new AuthorizationManager(_db);
                Session session = authorizationManager.ValidateAndUpdateSession(request.Token);
                if (session == null)
                {
                    return Content(System.Net.HttpStatusCode.Unauthorized, "Invalid session");
                }

                // Create launch payload for client to submit
                ILaunchManager launchManager = new LaunchManager(_db);
                LaunchData launchResponse;
                try
                {
                    launchResponse = launchManager.SignLaunch(session, request.AppId);
                }
                catch(ArgumentException e)
                {
                    return Content(System.Net.HttpStatusCode.NotFound, "Application not found");
                }

                // Save updated (extended) session
                try
                {
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException)
                {
                    _db.Entry(session).State = System.Data.Entity.EntityState.Detached;
                    return InternalServerError();
                }

                return Ok(launchResponse);
            }
        }
    }
}
