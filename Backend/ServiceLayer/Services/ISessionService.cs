using DataAccessLayer.Models;
using DataAccessLayer.Database;
using System.Collections.Generic;
using System;

namespace ServiceLayer.Services
{
    public interface ISessionService
    {
        Session CreateSession(DatabaseContext _db, Session session);
        Session GetSession(DatabaseContext _db, string token);
        Session UpdateSession(DatabaseContext _db, Session session);
        Session DeleteSession(DatabaseContext _db, string  token);
        List<Session> GetSessions(DatabaseContext _db, Guid userId);
    }
}
