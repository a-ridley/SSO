using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class UserManagementRepository
    {
        DatabaseContext _db;
        public UserManagementRepository(DatabaseContext _db)
        {
            this._db = _db;
        }

        public User CreateNewUser(User user)
        {
            _db.Entry(user).State = EntityState.Added;
            return user;
        }

        public User DeleteUser(Guid Id)
        {
            var user = _db.Users
                .Where(c => c.Id == Id)
                .FirstOrDefault<User>();
            if (user == null)
                return null;
            _db.Entry(user).State = EntityState.Deleted;
            return user;
        }

        public User GetUser(string email)
        {
            var user = _db.Users
                .Where(c => c.Email == email)
                .FirstOrDefault<User>();
            return user;
        }

        public User GetUser(Guid Id)
        {
            return _db.Users.Find(Id);
        }

        public User UpdateUser(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _db.Entry(user).State = EntityState.Modified;
            return user;
        }

        public bool ExistingUser(User user)
        {
            var result = GetUser(user.Email);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public bool ExistingUser(string email)
        {
            var result = GetUser(email);
            if (result != null)
            {
                return true;
            }
            return false;
        }
    }
}
