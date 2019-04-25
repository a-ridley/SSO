using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using DataAccessLayer.Models;
using System.Security.Cryptography;
using DataAccessLayer.Database;
using ServiceLayer.Services;
using Newtonsoft.Json;

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


    public class LogoutManager
    { 
        DatabaseContext _db;
        UserService userService;
        SessionService sessionServ;
        public LogoutManager(DatabaseContext _db)
        {
            this._db = _db;
            userService = new UserService(_db);
            _applicationService = new ApplicationService(_db);
            sessionServ = new SessionService(_db);


        }
        IApplicationService _applicationService;
        public async Task<HttpResponseMessage> SendLogoutRequest(string token)
        {
            var applist = _applicationService.GetAllApplications();
            Session session = sessionServ.GetSession(token);
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

                var stringPayload = JsonConvert.SerializeObject(logoutPayload);
                var jsonPayload = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                var request = await client.PostAsync(app.LogoutUrl, jsonPayload);
                if(request != null)
                {
                    return request;
                }

            }
            return null;
        }

    }
}