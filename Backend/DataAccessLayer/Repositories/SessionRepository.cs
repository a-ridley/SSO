using DataAccessLayer.Models;
using DataAccessLayer.Database;
using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        DatabaseContext _db;
        public SessionRepository(DatabaseContext _db)
        {
            this._db = _db;
        }

        public Session GetSession(string token)
        {
            var session = _db.Sessions
                .Where(s => s.Token == token)
                .FirstOrDefault<Session>();

            return session;
        }
        
        public Session CreateSession(Session session)
        {
            _db.Sessions.Add(session);
            return session;
        }

        public Session UpdateSession(Session session)
        {
            session.UpdatedAt = DateTime.UtcNow;
            session.ExpiresAt = DateTime.UtcNow.AddMinutes(Session.MINUTES_UNTIL_EXPIRATION);
            _db.Entry(session).State = EntityState.Modified;
            return session;
        }

        public Session DeleteSession(string token)
        {
            var session = _db.Sessions
                .Where(s => s.Token == token)
                .FirstOrDefault<Session>();
            if (session == null)
                return null;

            _db.Sessions.Remove(session);
            return session;
        }

        public List<Session> DeleteSessions(Guid userId)
        {
            var sessions = _db.Sessions
                .Where(s => s.UserId == userId)
                .ToList();

            if (sessions == null)
                return null;

            _db.Sessions.RemoveRange(sessions);
            return sessions;
                
        }

        public List<Session> GetSessions(Guid userId)
        {
            var sessions = _db.Sessions
                .Where(s => s.UserId == userId)
                .ToList();

            return sessions;
        }
    }
}
