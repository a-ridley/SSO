using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using ServiceLayer.Services;
using DataAccessLayer.Database;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using DataAccessLayer.Requests;

namespace ManagerLayer.Login
{
    public class LoginManager
    {
        IPasswordService _passwordService = new PasswordService();
        UserRepository userRepo = new UserRepository();

        private User user;
        private ITokenService _tokenService = new TokenService();

        public LoginManager()
        {
            
        }

        public bool LoginCheckUserExists(string email)
        {
            using (var _db = new DatabaseContext())
            {
                IUserService _userService = new UserService(_db);
                user = _userService.GetUser(email);
                if (user != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public bool LoginCheckUserDisabled(string email)
        {
            using (var _db = new DatabaseContext())
            {
                IUserService _userService = new UserService(_db);
                user = _userService.GetUser(email);
                if (user.Disabled)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool LoginCheckPassword(LoginRequest request)
        {
            bool result;
            using (var _db = new DatabaseContext())
            {
                IUserService _userService = new UserService(_db);
                user = _userService.GetUser(request.email);
                string hashedPassword = _passwordService.HashPassword(request.password, user.PasswordSalt);
                if (userRepo.ValidatePassword(user, hashedPassword))
                {
                    user.IncorrectPasswordCount = 0;
                    result = true;
                }
                else
                {
                    user.IncorrectPasswordCount = ++user.IncorrectPasswordCount;
                    if (user.IncorrectPasswordCount == 3)
                    {
                        user.Disabled = true;
                    }
                    result = false;
                }

                try
                {
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    _db.Entry(user).State = System.Data.Entity.EntityState.Detached;
                }
            }
            return result;
        }

        public string LoginAuthorized(string email)
        {
            using (var _db = new DatabaseContext())
            {
                IUserService _userService = new UserService(_db);
                user = _userService.GetUser(email);
                string generateToken = _tokenService.GenerateToken();
                Session session = new Session
                {
                    Token = generateToken,
                    UserId = user.Id
                };

                ISessionService _sessionService = new SessionService(_db);
                var response = _sessionService.CreateSession(session);
                try
                {
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    _db.Entry(session).State = System.Data.Entity.EntityState.Detached;
                }
                return session.Token;
            }
        }
    }
}
