using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private UserManagementRepository _userManagementRepo;

        public UserService(DatabaseContext _db)
        {
            _userManagementRepo = new UserManagementRepository(_db);
        }

        public User CreateUser(User user)
        {
            if (_userManagementRepo.ExistingUser(user.Email))
            {
                throw new ArgumentException("A user with that email address already exists");
            }
            return _userManagementRepo.CreateNewUser(user);
        }

        public User DeleteUser(Guid Id)
        {
            return _userManagementRepo.DeleteUser(Id);
        }

        public User GetUser(string email)
        {
            return _userManagementRepo.GetUser(email);
        }

        public User GetUser(Guid Id)
        {
            return _userManagementRepo.GetUser(Id);
        }

        public User UpdateUser(User user)
        {
            return _userManagementRepo.UpdateUser(user);
        }

        public User Login(string email, string password)
        {
            UserRepository userRepo = new UserRepository();
            PasswordService _passwordService = new PasswordService();
            var user = _userManagementRepo.GetUser(email);
            if (user != null)
            {
                string hashedPassword = _passwordService.HashPassword(password, user.PasswordSalt);
                if (userRepo.ValidatePassword(user, hashedPassword))
                {
                    Console.WriteLine("Password Correct");
                    return user;
                }
                Console.WriteLine("Password Incorrect");
                return null;
            }
            Console.WriteLine("User does not exist");
            return null;
        }

        public bool ExistingUser(string email)
        {
            return _userManagementRepo.ExistingUser(email);
        }
    }
}
