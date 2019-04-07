using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public interface ISessionRepository
    {
        Session GetSession(DatabaseContext _db, string token);
        Session CreateSession(DatabaseContext _db, Session session);
        Session UpdateSession(DatabaseContext _db, Session session);
        Session DeleteSession(DatabaseContext _db, string token);
        List<Session> GetSessions(DatabaseContext _db, Guid userId);
    }
}
