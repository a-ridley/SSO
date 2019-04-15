using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Exceptions;
using ServiceLayer.Services;
using System;
using System.Data.Entity.Infrastructure;
using System.Security.Cryptography;

namespace ManagerLayer
{
    public class AuthorizationManager : IAuthorizationManager
    {
        private ISessionService _sessionService;

        private DatabaseContext _db;

        public AuthorizationManager(DatabaseContext _db)
        {
            this._db = _db;
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
            if (response == null)
            {
                return null;
            }
			if (response.ExpiresAt > DateTime.UtcNow)
			{
				return _sessionService.UpdateSession(response);
			}
			else 
			{
				try
				{
					_sessionService.DeleteSession(token);
                    _db.SaveChanges();
					return null;
				}
				catch (DbUpdateException ex)
				{
					if (ex.InnerException == null)
					{
						throw;
					}
					else
					{
						//Log?
						var decodeErrors = ex.Entries;
						return null;
					}

				}
			
			}
		}

		public Session DeleteSession(string token)
        {
            return _sessionService.DeleteSession(token);
        }
    }
}
