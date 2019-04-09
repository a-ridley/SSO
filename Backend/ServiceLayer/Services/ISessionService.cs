using DataAccessLayer.Models;

namespace ServiceLayer.Services
{
    public interface ISessionService
    {
        Session CreateSession(Session session);
        Session GetSession(string token);
        Session UpdateSession(Session session);
        Session DeleteSession(string token);
    }
}
