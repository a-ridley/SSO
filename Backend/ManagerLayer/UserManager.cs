using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System.Data.Entity.Validation;
using ServiceLayer.Exceptions;
using ServiceLayer.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer
{
    public class UserManager
    {
        IApplicationService _applicationService;
        PasswordService _passwordService;
        UserService _userService;
        public UserManager(DatabaseContext _db)
        {
            this._passwordService = new PasswordService();
            this._userService = new UserService(_db);
            _applicationService = new ApplicationService(_db);
        }

        public User CreateUser(
            string email,
            string password,
            DateTime dob,
            string city,
            string state,
            string country,
            string securityQ1,
            string securityQ1Answer,
            string securityQ2,
            string securityQ2Answer,
            string securityQ3,
            string securityQ3Answer)
        {
            new System.Net.Mail.MailAddress(email);

            DateTime today18YearsAgo = DateTime.Now.AddYears(-18);
            if (dob > today18YearsAgo)
            {
                throw new InvalidDobException("Date of birth less than 18 years ago");
            }

            if (!_passwordService.CheckPasswordLength(password))
            {
                throw new PasswordInvalidException("Password is too short");
            }

            int pwnedCount = _passwordService.CheckPasswordPwned(password);

            if (pwnedCount > 0)
            {
                throw new PasswordPwnedException("Password has been pwned");
            }

            byte[] salt = _passwordService.GenerateSalt();
            string hash = _passwordService.HashPassword(password, salt);

            User user = new User
            {
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,

                DateOfBirth = dob,
                City = city,
                State = state,
                Country = country,

                SecurityQ1 = securityQ1,
                SecurityQ1Answer = securityQ1Answer,
                SecurityQ2 = securityQ2,
                SecurityQ2Answer = securityQ2Answer,
                SecurityQ3 = securityQ3,
                SecurityQ3Answer = securityQ3Answer,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            return _userService.CreateUser(user);
        }


        /// <summary>
        ///     Not part of spring 3
        ///     - in development
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public void Login(string email, string password)
        {
            //UserService _userService = new UserService();
            //PasswordService _passwordService = new PasswordService();
            //var user = _userService.Login(email, password);
        }

        public User GetUser(Guid userId)
        {
            return _userService.GetUser(userId);
        }


        public async Task<User> DeleteUser(DatabaseContext _db, Guid userId)
        {
            UserDeleteService uds = new UserDeleteService();
            IUserService _userService = new UserService(_db);
            SignatureService _signatureService = new SignatureService();
            ISessionService _sessionService = new SessionService(_db);
            User deletingUser = _userService.GetUser(userId);
            var sessions = _sessionService.GetSessions(userId);
            var applications = _applicationService.GetAllApplicationsList();
            var responseList = new List<HttpResponseMessage>();
            //iterate through each application to check health, if any app is down, do not run delete process
            foreach(Application app in applications)
            {
                if (app.Title != "Broadway Builder")
                {
                    var httpresponse = await _applicationService.GetApplicationHealth(app.HealthCheckUrl);
                    if (!httpresponse.IsSuccessStatusCode)
                    {
                        throw new FailHealthCheckException("Failed to delete, an application is down");
                    }
                }
            }
            var responses = "";
            foreach (Application app in applications)
            {
                var deletePayload = new Dictionary<string, string>();
                deletePayload.Add("ssoUserId", userId.ToString());
                deletePayload.Add("email", deletingUser.Email);
                deletePayload.Add("timestamp", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                var signature = _signatureService.Sign(app.SharedSecretKey, deletePayload);
                deletePayload.Add("signature", signature);
                var request = await uds.SendDeleteRequest(app.UserDeletionUrl,deletePayload);
                responseList.Add(request);
                var message = await request.Content.ReadAsStringAsync();
                responses = responses + "(" + app.Title + ": " + request.StatusCode.ToString() + ", " + message + ") ";
            }
            if (responseList.All(response => response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound))
            {
                User deletedUser = _userService.DeleteUser(userId);
                if(deletedUser != null){
                    _sessionService.DeleteSessions(deletedUser.Id);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        _db.Entry(deletedUser).State = System.Data.Entity.EntityState.Detached;
                    }
                }
                return deletedUser; // delete successful 
            }
            throw new FailedDeleteException("Some applications failed to delete." + responses); // some application(s) sent back a non 200 or 404 reponse
        }
    }
}
