using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace ServiceLayer.Services
{
    public class SessionService : ISessionService
    {
        private ISessionRepository _SessionRepo;

        public SessionService(DatabaseContext _db)
        {
            _SessionRepo = new SessionRepository(_db);
        }

        public Session CreateSession(Session session)
        {
            return _SessionRepo.CreateSession(session);
        }

        public Session GetSession(string token)
        {
            return _SessionRepo.GetSession(token);
        }

        public Session UpdateSession(Session session)
        {
            return _SessionRepo.UpdateSession(session);
        }

        public Session DeleteSession(string token)
        {
            return _SessionRepo.DeleteSession(token);
        }
    }
}
