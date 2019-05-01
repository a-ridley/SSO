using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using DataAccessLayer.Models;
using DataAccessLayer.Database;
using ServiceLayer.Services;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace ManagerLayer.Logout
{
   


    public class LogoutManager
    { 
        DatabaseContext _db;
        UserService userService;
        SessionService sessionServ;
        Dictionary<Application,Boolean> checkResult = new Dictionary<Application,Boolean>();
        
        public LogoutManager(DatabaseContext _db)
        {
            this._db = _db;
            userService = new UserService(_db);
            _applicationService = new ApplicationService(_db);
            sessionServ = new SessionService(_db);


        }
        IApplicationService _applicationService;
        public Task<Dictionary<Application,Boolean>> SendLogoutRequest(string token)
        {
            var applist = _applicationService.GetAllApplications();
            Session session = sessionServ.GetSession(token);
            foreach (Application app in applist)
            {
                checkResult.Add(app,false);
            }
            foreach (Application app in applist)
            {
                User user = userService.GetUser(session.UserId);
                HttpClient client = new HttpClient();

                //The logoutpayload will have a dictonary that represents signed body of request.
                var logoutPayload = new Dictionary<string, string>();
                logoutPayload.Add("ssoUserId", session.UserId.ToString());
                logoutPayload.Add("email", user.Email);
                logoutPayload.Add("timestamp", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());

                //This signs the dictonary and adds it back to the payload.
                var signatureService = new SignatureService();
                var signature = signatureService.Sign(app.SharedSecretKey, logoutPayload);
                logoutPayload.Add("signature", signature);

                //This converts payload to JSON and sends it to each application logout URL.
                var stringPayload = JsonConvert.SerializeObject(logoutPayload);
                var jsonPayload = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                var request = client.PostAsync(app.LogoutUrl, jsonPayload);
                if(request.Status.Equals(200))
                {
                    checkResult[app] = false;
                    return Task.FromResult(checkResult);
                }
               

            }
            
            return null;
        }

    }
}