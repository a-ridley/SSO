using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;

namespace ManagerLayer.LaunchManagement
{
    // Creates a signed launch ready to be submitted to applications
    public class LaunchManager : ILaunchManager
    {
        DatabaseContext _db;
        UserService userService;
        public LaunchManager(DatabaseContext _db)
        {
            this._db = _db;
            userService = new UserService(_db);
        }

        // Creates and signs a payload to launch to a given application
        public LaunchData SignLaunch(Session session, Guid appId)
        {
            Application app = ApplicationService.GetApplication(_db, appId);

            if (app == null)
            {
                throw new ArgumentException();
            }

            User user = userService.GetUser(session.UserId);

            // Dictionary represents the signed body of the request to the destination server
            // Props can be added to this, and they will be added to signature
            var launchPayload = new Dictionary<string, string>();
            launchPayload.Add("ssoUserId", session.UserId.ToString());
            launchPayload.Add("email", user.Email);
            launchPayload.Add("timestamp", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());

            // Sign the dictionary and add the resulting signature back
            // SECURITY: This step is important for the validity of the payload
            var signatureService = new SignatureService();
            var signature = signatureService.Sign(app.SharedSecretKey, launchPayload);

            launchPayload.Add("signature", signature);

            // Include app URL for routing purposes
            var launchData = new LaunchData
            {
                launchPayload = launchPayload,
                url = app.LaunchUrl
            };

            return launchData;
        }
    }
}
