using DataAccessLayer.Models;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public interface ISessionRepository
    {
        Session GetSession(string token);
        Session CreateSession(Session session);
        Session UpdateSession(Session session);
        Session DeleteSession(string token);
        List<Session> GetSessions(Guid userId);
    }
}
