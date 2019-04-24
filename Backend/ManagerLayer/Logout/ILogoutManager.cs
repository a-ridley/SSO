using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;

namespace ManagerLayer.Logout
{
    public interface ILogoutManager
    {
        LogoutResponse SendLogoutRequest(Session session);
    }
}
