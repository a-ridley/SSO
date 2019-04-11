using System;
using ManagerLayer.UserManagement;
using ManagerLayer.Login;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLayer.Models;

namespace UnitTesting
{

    [TestClass]
    public class LoginManagerUT
    {

        LoginManager lm;
        UserManagementManager um;
        LoginRequest request;
        TestingUtils tu;

        public LoginManagerUT()
        {
            lm = new LoginManager();
            um = new UserManagementManager();
            request = new LoginRequest();
            tu = new TestingUtils();
        }

        //LoginCheckUserExists()
        [TestMethod]
        public void LoginCheckUserExists_Success_ReturnTrue()
        {
            // Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = newUser.Email;
            request.password = "qwertyuiop_!2019";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                result = lm.LoginCheckUserExists(request);

                // Assert
                Assert.AreEqual(true, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void LoginCheckUserExists_Fail_ReturnTrue()
        {            
            // Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = "doesnotexist@gmail.com";
            request.password = "qwertyuiop_!2019";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                result = lm.LoginCheckUserExists(request);

                // Assert
                Assert.AreNotEqual(true, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void LoginCheckUserExists_Success_ReturnFalse()
        {
            // Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = "doesnotexist@gmail.com";
            request.password = "qwertyuiop_!2019";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                result = lm.LoginCheckUserExists(request);

                // Assert
                Assert.AreEqual(false, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void LoginCheckUserExists_Fail_ReturnFalse()
        {
            // Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = "doesnotexist@gmail.com";
            request.password = "qwertyuiop_!2019";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                result = lm.LoginCheckUserExists(request);

                // Assert
                Assert.AreEqual(false, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        //LoginCheckUserDisabled()
        [TestMethod]
        public void LoginCheckUserDisabled_Success_ReturnTrue()
        {
            //Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = "cf2080@icloud.com";
            request.password = "qwertyuiop_!2019";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                newUser.Disabled = true;
                um.UpdateUser(newUser);
                _db.SaveChanges();
                result = lm.LoginCheckUserDisabled(request);

                // Assert
                Assert.AreEqual(true, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void LoginCheckUserDisabled_Fail_ReturnTrue()
        {
            //Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = "cf2080@icloud.com";
            request.password = "qwertyuiop_!2019";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                result = lm.LoginCheckUserDisabled(request);

                // Assert
                Assert.AreNotEqual(true, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void LoginCheckUserDisabled_Success_ReturnFalse()
        {
            //Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = "cf2080@icloud.com";
            request.password = "qwertyuiop_!2019";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                result = lm.LoginCheckUserDisabled(request);

                // Assert
                Assert.AreEqual(false, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void LoginCheckUserDisabled_Fail_ReturnFalse()
        {
            //Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = "cf2080@icloud.com";
            request.password = "qwertyuiop_!2019";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                newUser.Disabled = true;
                um.UpdateUser(newUser);
                _db.SaveChanges();
                result = lm.LoginCheckUserDisabled(request);

                // Assert
                Assert.AreNotEqual(false, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        //LoginCheckPassword
        [TestMethod]
        public void LoginCheckPassword_Success_ReturnTrue()
        {
            //Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = "cf2080@icloud.com";
            request.password = "qwertyuiop136_!2019";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                result = lm.LoginCheckPassword(request);

                // Assert
                Assert.AreEqual(true, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void LoginCheckPassword_Fail_ReturnTrue()
        {            
            //Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = "cf2080@icloud.com";
            request.password = "qwertyuiop136_!2019!";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                result = lm.LoginCheckPassword(request);

                // Assert
                Assert.AreNotEqual(true, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void LoginCheckPassword_Success_ReturnFalse()
        {
            //Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = "cf2080@icloud.com";
            request.password = "qwertyuiop136_!2019!";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                result = lm.LoginCheckPassword(request);

                // Assert
                Assert.AreEqual(false, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void LoginCheckPassword_Fail_ReturnFalse()
        {
            //Arrange
            User newUser = tu.CreateUserInDbManager();
            bool result;
            request.email = "cf2080@icloud.com";
            request.password = "qwertyuiop136_!2019";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                result = lm.LoginCheckPassword(request);

                // Assert
                Assert.AreNotEqual(false, result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }

        // LoginAuthorized()
        [TestMethod]
        public void LoginAuthorized_Success_ReturnToken()
        {
            //Arrange
            User newUser = tu.CreateUserInDbManager();
            string result;
            request.email = "cf2080@icloud.com";
            request.password = "qwertyuiop136_!2019";

            using (var _db = tu.CreateDataBaseContext())
            {
                // Act 
                result = lm.LoginAuthorized(request);

                // Assert
                Assert.IsNotNull(result);
                um.DeleteUser(newUser.Id);
                _db.SaveChanges();
            }
        }
    }
}
