using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ManagerLayer.Login;
using ManagerLayer.Logout;
using System;
using System.Data.Entity.Validation;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using ManagerLayer;
using ServiceLayer.Exceptions;
using System.ComponentModel.DataAnnotations;
using KFC_WebAPI.RequestModels;
using ManagerLayer.PasswordManagement;
using ServiceLayer.Services;
using ManagerLayer.UserManagement;
using DataAccessLayer.Requests;
using System.Data.Entity.Infrastructure;
using System.Net.Http;

namespace KFC_WebAPI.Controllers
{
    public class UserDeleteRequest
    {
        [Required]
        public string token { get; set; }
    }

    public class UserDeleteRequestFromApp
    {
        // Included in signature
        public string appId { get; set; }
        public Guid ssoUserId { get; set; }
        public string email { get; set; }
        public long timestamp { get; set; }

        // Excluded from signature
        public string signature { get; set; }

        // Generate string to be signed
        public string PreSignatureString()
        {
            string acc = "";
            acc += "ssoUserId=" + ssoUserId + ";";
            acc += "email=" + email + ";";
            acc += "timestamp=" + timestamp + ";";
            return acc;
        }
    }

    public class UsersController : ApiController
    {
        [HttpPost]
        [Route("api/users/register")]
        public IHttpActionResult Register([FromBody, Required]UserRegistrationRequest request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return Content((HttpStatusCode)412, ModelState);
            }

            using (var _db = new DatabaseContext())
            {
                User user;
                try
                {
                    UserManager userManager = new UserManager(_db);
                    user = userManager.CreateUser(
                        request.email,
                        request.password,
                        request.dob,
                        request.city,
                        request.state,
                        request.country,
                        request.securityQ1,
                        request.securityQ1Answer,
                        request.securityQ2,
                        request.securityQ2Answer,
                        request.securityQ3,
                        request.securityQ3Answer);
                }
                catch (ArgumentException)
                {
                    return Conflict();
                }
                catch (FormatException)
                {
                    return Content((HttpStatusCode)406, "Invalid email address.");
                }
                catch (PasswordInvalidException)
                {
                    return Content((HttpStatusCode)401, "That password is too short. Password must be between 12 and 2000 characters.");
                }
                catch (PasswordPwnedException)
                {
                    return Content((HttpStatusCode)401, "That password has been hacked before. Please choose a more secure password.");
                }
                catch (InvalidDobException)
                {
                    return Content((HttpStatusCode)401, "This software is intended for persons over 18 years of age.");
                }

                AuthorizationManager authorizationManager = new AuthorizationManager(_db);
                Session session = authorizationManager.CreateSession(user);
                try
                {
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException)
                {
                    _db.Entry(user).State = System.Data.Entity.EntityState.Detached;
                    _db.Entry(session).State = System.Data.Entity.EntityState.Detached;
                    return InternalServerError();
                }

                return Ok(new
                {
                    token = session.Token
                });
            }
        }

        [HttpGet]
        [Route("api/users/{token}")]
        public string GetEmail(string token)
        {
            UserManagementManager umm = new UserManagementManager();
            Session session = new Session();
            User user;
            string email;

            try
            {
                using (var _db = new DatabaseContext())
                {
                    SessionService ss = new SessionService(_db);
                    session = ss.GetSession(token);
                    Console.WriteLine(session);
                }

                var id = session.UserId;
                user = umm.GetUser(id);
                email = user.Email;

            }
            catch(ArgumentNullException)
            {
                throw new ArgumentNullException("Session is null");
            }

            return email;
        }

        [HttpPost]
        [Route("api/users/login")]
        public IHttpActionResult Login([FromBody] LoginRequest request)
        {
            LoginManager loginM = new LoginManager();
            if (loginM.LoginCheckUserExists(request.email) == false)
            {
                //400
                return Content(HttpStatusCode.BadRequest, "Invalid Username");
            }

            if (loginM.LoginCheckUserDisabled(request.email))
            {
                //401
                return Content(HttpStatusCode.Unauthorized, "User is Disabled");
            }

            if (loginM.LoginCheckPassword(request))
            {
                return Ok(loginM.LoginAuthorized(request.email));
            }
            else
            {
                //400
                return Content(HttpStatusCode.BadRequest, "Invalid Password");
            }
        }

        [HttpPost]
        [Route("api/users/updatepassword")]
        public IHttpActionResult UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            using (var _db = new DatabaseContext())
            {
                PasswordManager pm = new PasswordManager(_db);
                int result = pm.UpdatePasswordController(request);
                if (result == 1)
                {
                    return Content(HttpStatusCode.OK, "Password has been updated");
                }
                else if (result == -1)
                {
                    return Content(HttpStatusCode.BadRequest, "New password matches old password");
                }
                else if (result == -2)
                {
                    return Content(HttpStatusCode.BadRequest, "Password has been pwned, please use a different password");
                }
                else if (result == -3)
                {
                    return Content(HttpStatusCode.BadRequest, "New password does not meet minimum password requirements");
                }
                else if (result == -4)
                {
                    return Content(HttpStatusCode.BadRequest, "Current password inputted does not match current password");
                }
                else if (result == -5)
                {
                    return Content(HttpStatusCode.Unauthorized, "Session invalid");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "Service Unavailable");
                }
            }
        }


        [HttpPost]
        [Route("api/users/deleteuser")]
        public async Task<IHttpActionResult> Delete([FromBody] UserDeleteRequest request)
        {
            using (var _db = new DatabaseContext())
            {
                IAuthorizationManager authorizationManager = new AuthorizationManager(_db);
                Session session = authorizationManager.ValidateAndUpdateSession(request.token);
                if (session == null)
                {
                    return Content(HttpStatusCode.Unauthorized, "Session not valid!");
                }
                UserManager um = new UserManager(_db);
                User user = await um.DeleteUser(_db, session.UserId);
                if (user != null)
                {
                    return Content(HttpStatusCode.OK, "Account has been deleted");
                }
                return Content(HttpStatusCode.BadRequest, "Service Unavailable");
            }
        }

        [HttpPost]
        [Route("api/users/appdeleteuser")]
        public async Task<IHttpActionResult> Delete([FromBody] UserDeleteRequestFromApp request)
        {
            using (var _db = new DatabaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db);
                HttpClient client = new HttpClient();
                ITokenService _tokenService = new TokenService();
                Application app = _applicationService.GetApplication(Guid.Parse(request.appId));
                if (!(_tokenService.GenerateSignature(request.PreSignatureString(), app) == request.signature))
                {
                    return Content(HttpStatusCode.Unauthorized, "Signature not valid!");
                }
                UserManager um = new UserManager(_db);
                User deletedUser = await um.DeleteUser(_db, request.ssoUserId);
                if(deletedUser != null)
                {
                    return Content(HttpStatusCode.OK, "Account has been deleted");
                }
                return Content(HttpStatusCode.BadRequest, "Service Unavailable");
            }
        }

        [HttpPost]
        [Route("api/Logout")]
        public IHttpActionResult Logout([FromBody] LogoutRequest request)
        {
            using (var _db = new DatabaseContext())
            {
                SessionService serv = new SessionService(_db);
                IAuthorizationManager authorizationManager = new AuthorizationManager(_db);
                LogoutManager logoutManager = new LogoutManager(_db);
                Session session = authorizationManager.ValidateAndUpdateSession(request.token);
                if (session == null)
                {
                    return Content(HttpStatusCode.Unauthorized, "Session not valid!");
                }
                try
                {
                    var appresponse = logoutManager.SendLogoutRequest(request.token);
                    if (appresponse != null)
                    {
                        return Content(HttpStatusCode.BadRequest, "Service is not available: " + appresponse + " shas not properly logged out");
                      
                    }
                    
                }
                catch
                {
                    return Content(HttpStatusCode.InternalServerError, "There was an error on the server and the request could not be completed");
                }
                
                try
                {
                    var response = authorizationManager.DeleteSession(request.token);
                    _db.SaveChanges();

                    if (response != null)
                    {

                        return Ok("User has logged out");
                    }

                }
                catch (DbUpdateException)
                {
                    return Content(HttpStatusCode.InternalServerError, "There was an error on the server and the request could not be completed");
                }
                return Content(HttpStatusCode.ExpectationFailed, "Session has not been found.");

            }

        }

    }
}
