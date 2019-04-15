using DataAccessLayer.Models;
using System.Collections.Generic;
using System;

namespace ServiceLayer.Services
{
    public interface ISessionService
    {
        Session CreateSession(Session session);
        Session GetSession(string token);
        Session UpdateSession(Session session);
        Session DeleteSession(string token);
        List<Session> GetSessions(Guid userId);
    }
}
