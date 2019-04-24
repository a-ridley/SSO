using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using DataAccessLayer.Models;
using System.Security.Cryptography;
using DataAccessLayer.Database;
using ServiceLayer.Services;

namespace ManagerLayer.Logout
{
    public class UserLogoutPayload
    {
        // Included in signature
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
    public class LogoutResponse
    {
        public string url { get; set; }
        public UserLogoutPayload logoutPayload{ get; set; }
    }

    public class LogoutManager : ILogoutManager
    {
        DatabaseContext _db;
        UserService userService;
        public LogoutManager(DatabaseContext _db)
        {
            this._db = _db;
            userService = new UserService(_db);
            _applicationService = new ApplicationService(_db);


        }
        IApplicationService _applicationService;
        public LogoutResponse SendLogoutRequest(Session session)
        {
            var applist = _applicationService.GetAllApplications();
            foreach (Application app in applist)
            {
                User user = userService.GetUser(session.UserId);
                HttpClient client = new HttpClient();
                UserLogoutPayload logoutPayload = new UserLogoutPayload
                {
                    email = user.Email,
                    ssoUserId = session.UserId,
                    timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
                };
                HMACSHA256 hmacsha1 = new HMACSHA256(Encoding.ASCII.GetBytes(app.SharedSecretKey));
                byte[] logoutPayloadBuffer = Encoding.ASCII.GetBytes(logoutPayload.PreSignatureString());
                byte[] signatureBytes = hmacsha1.ComputeHash(logoutPayloadBuffer);
                logoutPayload.signature = Convert.ToBase64String(signatureBytes);

                return new LogoutResponse
                {
                    logoutPayload = logoutPayload,
                    url = app.LaunchUrl
                };
            }
            return null;
        }

    }
}