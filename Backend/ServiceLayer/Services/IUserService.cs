using DataAccessLayer.Models;
using System;

namespace ServiceLayer.Services
{
    public interface IUserService
    {
        User CreateUser(User user);
        User GetUser(string email);
        User GetUser(Guid Id);
        User DeleteUser(Guid Id);
        User UpdateUser(User user);
        bool ExistingUser(string email);
    }
}
