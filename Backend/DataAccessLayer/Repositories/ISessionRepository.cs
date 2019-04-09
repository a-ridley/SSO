using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories
{
    public interface ISessionRepository
    {
        Session GetSession(string token);
        Session CreateSession(Session session);
        Session UpdateSession(Session session);
        Session DeleteSession(string token);
    }
}
