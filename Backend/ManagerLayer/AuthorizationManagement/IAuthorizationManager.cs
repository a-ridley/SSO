using DataAccessLayer.Database;
using DataAccessLayer.Models;

namespace ManagerLayer
{
    public interface IAuthorizationManager
    {
        string GenerateSessionToken();
        Session CreateSession(User user);
        Session ValidateAndUpdateSession(string token);
        Session DeleteSession(string token);
    }
}
