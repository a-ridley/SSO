using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Security.Cryptography;

namespace ManagerLayer
{
    public class AuthorizationManager : IAuthorizationManager
    {
        private ISessionService _sessionService;

        public AuthorizationManager(DatabaseContext _db)
        {
             _sessionService = new SessionService(_db);
        }

        public string GenerateSessionToken()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            Byte[] b = new byte[64 / 2];
            provider.GetBytes(b);
            return BitConverter.ToString(b).Replace("-", "").ToLower();
        }

        public Session CreateSession(User user)
        {
            Session session = new Session();
            session.Token = GenerateSessionToken();
            session.UserId = user.Id;

            var response = _sessionService.CreateSession(session);

            return session;
        }

        public Session ValidateAndUpdateSession(string token)
        {
            Session response = _sessionService.GetSession(token);

            if(response != null)
            {
                return _sessionService.UpdateSession(response);
            }
            else
            {
                return null;
            }
        }

        public Session DeleteSession(string token)
        {
            return _sessionService.DeleteSession(token);
        }
    }
}
