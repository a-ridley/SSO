using System;
using System.Data.Entity.Validation;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ManagerLayer.UserManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;

namespace UnitTesting
{
    [TestClass]
    public class UserManagementManagerUT
    {
        TestingUtils tu;

        public UserManagementManagerUT()
        {
            tu = new TestingUtils();
        }

        [TestMethod]
        public void Disable_User_Success()
        {
            // Arrange
            User newUser = tu.CreateUserInDb();
            var expectedResponse = newUser;
            var expectedResult = true;

            // ACT
            UserManagementManager _umm = new UserManagementManager();
            var response = _umm.DisableUser(newUser);
            var result = _umm.GetUser(newUser.Id);

            // Assert
            Assert.IsTrue(response == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result.Disabled);
        }

        [TestMethod]
        public void Enable_User_Success()
        {
            // Arrange
            User newUser = tu.CreateUserInDb();
            var expectedResponse = newUser;
            var expectedResult = false;

            // ACT
            UserManagementManager _umm = new UserManagementManager();
            var response = _umm.EnableUser(newUser);
            var result = _umm.GetUser(newUser.Id);

            // Assert
            Assert.IsTrue(response == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result.Disabled);
        }

        [TestMethod]
        public void Toggle_User_Success()
        {
            // Arrange
            User newUser;
            using (var _db = tu.CreateDataBaseContext())
            {
                newUser = tu.CreateUserObject();
                _db.Users.Add(newUser);
                _db.SaveChanges();
            }
            var expectedResponse = newUser;
            var expectedResult = newUser.Disabled;

            // ACT
            UserManagementManager _umm = new UserManagementManager();
            var response = _umm.ToggleUser(newUser, null);

            var result = _umm.GetUser(newUser.Id);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.Id, result.Id);
        }
    }
}
