using DataAccessLayer.Database;
using DataAccessLayer.Requests;
using ManagerLayer.PasswordManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;

namespace UnitTesting
{
    [TestClass]
    public class PasswordResetManagerUT
    {
        DatabaseContext _db;
        TestingUtils tu;
        PasswordManager pm;

        public PasswordResetManagerUT()
        {
            _db = new DatabaseContext();
            tu = new TestingUtils();
            pm = new PasswordManager();
        }

        [TestMethod]
        public void CreatePasswordReset_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            //Act
            var response = pm.CreatePasswordReset(newUser.Id);
            //Assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetPasswordReset_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var expected = pm.CreatePasswordReset(newUser.Id);
            //Act
            var response = pm.GetPasswordReset(expected.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(expected.ResetToken, response.ResetToken);
        }

        [TestMethod]
        public void UpdatePasswordReset_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var expected = pm.CreatePasswordReset(newUser.Id);
            //Act
            expected.ResetCount = 1;
            var response = pm.UpdatePasswordReset(expected);
            var actual = pm.GetPasswordReset(expected.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(expected.ResetToken, actual.ResetToken);
        }

        [TestMethod]
        public void DeletePasswordReset_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var expected = pm.CreatePasswordReset(newUser.Id);
            //Act
            var response = pm.DeletePasswordReset(expected.ResetToken);
            var result = pm.ExistingResetToken(expected.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetPasswordResetExpiration_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var expected = pm.CreatePasswordReset(newUser.Id);
            //Act
            var response = pm.GetPasswordResetExpiration(expected.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(expected.ExpirationTime, response);
        }

        [TestMethod]
        public void CheckExistingPasswordReset_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            //Act
            var response = pm.ExistingResetToken(newlyAddedPasswordReset.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response);
        }

        [TestMethod]
        public void GetAttemptsPerID_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            //Act
            var response = pm.GetAttemptsPerToken(newlyAddedPasswordReset.ResetToken);
            //Assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetPasswordResetStatus()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            //Act
            var response = pm.GetPasswordResetStatus(newlyAddedPasswordReset.ResetToken);
            //Assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void LockPasswordReset_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            //Act
            pm.LockPasswordReset(newlyAddedPasswordReset.ResetToken);
            var response = pm.GetPasswordReset(newlyAddedPasswordReset.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Disabled);
            Assert.IsFalse(response.AllowPasswordReset);
        }

        [TestMethod]
        public void CheckPasswordResetValid_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            //Act
            var response = pm.CheckPasswordResetValid(newlyAddedPasswordReset.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response);
        }

        [TestMethod]
        public void CountPasswordResetPast24Hours_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            //Act
            var response = pm.PasswordResetsMadeInPast24HoursByUser(newUser.Id);
            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(response, 1);
        }

        [TestMethod]
        public void CheckPasswordPwned_Pass()
        {
            //Arrange
            string password = "password";
            //Act
            var response = pm.CheckIsPasswordPwned(password);
            //Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response);
        }

        [TestMethod]
        public void SaltAndHashPassword_Pass()
        {
            //Arrange
            string password = "password";
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            string resetToken = newlyAddedPasswordReset.ResetToken;
            //Act
            var response = pm.SaltAndHashPassword(resetToken, password);
            //Assert
            Assert.IsNotNull(response);
            Assert.AreNotEqual(response, password);
        }

        [TestMethod]
        public void UpdatePassword__Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            string newPassword = "qwertyswag123";
            var newPasswordHashed = pm.HashPassword(newPassword, newUser.PasswordSalt);
            //Act
            var response = pm.UpdatePassword(newUser, newPasswordHashed);
            
            using(_db = tu.CreateDataBaseContext())
            {
                var us = new UserService(_db);
                var actual = us.GetUser(newUser.Email).PasswordHash;
                //Assert
                Assert.IsNotNull(response);
                Assert.IsTrue(response);
                Assert.AreEqual(newPasswordHashed, actual);
            }
        }

        [TestMethod]
        public void ResetPassword_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            string newPassword = "asdf";
            string newPasswordHashed = pm.HashPassword(newPassword, newUser.PasswordSalt);
            //Act
            var response = pm.ResetPassword(newlyAddedPasswordReset.ResetToken, newPasswordHashed);
            var actual = _db.Users.Find(newUser.Id).PasswordHash;
            //Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response);
            Assert.AreEqual(actual, newPasswordHashed);
        }

        [TestMethod]
        public void GetSecurityQuestions_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            string secQ1 = "Favorite food?";
            string secQ2 = "Favorite color?";
            string secQ3 = "Favorite hobby?";
            newUser.SecurityQ1 = secQ1;
            newUser.SecurityQ2 = secQ2;
            newUser.SecurityQ3 = secQ3;
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            //Act 
            var response = pm.GetSecurityQuestions(newlyAddedPasswordReset.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(response[0], secQ1);
            Assert.AreEqual(response[1], secQ2);
            Assert.AreEqual(response[2], secQ3);
        }

        [TestMethod]
        public void CheckSecurityAnswers_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            string secA1 = "Pizza";
            string secA2 = "Cyan";
            string secA3 = "Hiking";
            newUser.SecurityQ1Answer = secA1;
            newUser.SecurityQ2Answer = secA2;
            newUser.SecurityQ3Answer = secA3;
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            List<string> submittedAnswers = new List<string>
            {
                "Pizza",
                "Cyan",
                "Hiking"
            };
            //Act
            var response = pm.CheckSecurityAnswers(newlyAddedPasswordReset.ResetToken, submittedAnswers);
            //Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response);
        }

        [TestMethod]
        public void CheckIfPasswordResetAllowed_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            newlyAddedPasswordReset.AllowPasswordReset = true;
            pm.UpdatePasswordReset(newlyAddedPasswordReset);
            //Act
            var response = pm.CheckIfPasswordResetAllowed(newlyAddedPasswordReset.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response);
        }

        [TestMethod]
        public void CreatePasswordReset_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            //Act
            var expectedResetToken = "asdf";
            var actualResetToken = pm.CreatePasswordReset(newUser.Id);
            //Assert
            Assert.AreNotEqual(expectedResetToken, actualResetToken);
        }

        [TestMethod]
        public void GetPasswordReset_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var expected = pm.CreatePasswordReset(newUser.Id);
            var unexpected = pm.CreatePasswordReset(newUser.Id);
            //Act
            var response = pm.GetPasswordReset(unexpected.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.AreNotEqual(expected.ResetToken, response.ResetToken);
        }

        [TestMethod]
        public void UpdatePasswordReset_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var expected = pm.CreatePasswordReset(newUser.Id);
            //Act
            var response = pm.UpdatePasswordReset(expected);
            var actual = pm.GetPasswordReset(expected.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(expected.ResetToken, actual.ResetToken);
        }

        [TestMethod]
        public void DeletePasswordReset_Fail_NonexistingToken()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            string resetToken = "fakeResetToken";
            //Act
            var response = pm.DeletePasswordReset(resetToken);
            var result = pm.ExistingResetToken(resetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetPasswordResetExpiration_Fail_NonexistingToken()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            string resetToken = "fakeResetToken";
            //Act
            var response = pm.GetPasswordResetExpiration(resetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(DateTime.MinValue, response);
        }

        [TestMethod]
        public void CheckExistingPasswordReset_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            string resetToken = "fakeResetToken";
            //Act
            var response = pm.ExistingResetToken(resetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.IsFalse(response);
        }

        [TestMethod]
        public void GetPasswordResetStatus_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            //Act
            var response = pm.GetPasswordResetStatus(newlyAddedPasswordReset.ResetToken);
            //Assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void LockPasswordReset_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            string resetToken = "fakeResetToken";
            //Act
            var response = pm.GetPasswordReset(resetToken);
            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public void ResetPassword_Fail_InvalidToken()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            string resetToken = "fakeResetRoken";
            string newPassword = "asdf";
            //Act
            var response = pm.ResetPassword(resetToken, newPassword);
            var actual = _db.Users.Find(newUser.Id).PasswordHash;
            //Assert
            Assert.IsNotNull(response);
            Assert.IsFalse(response);
        }

        [TestMethod]
        public void GetSecurityQuestions_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();

            string secQ1 = "Favorite food?";
            string secQ2 = "Favorite color?";
            string secQ3 = "Favorite hobby?";
            newUser.SecurityQ1 = secQ1;
            newUser.SecurityQ2 = secQ2;
            newUser.SecurityQ3 = secQ3;
            tu.CreateUserInDb(newUser);
            string expectedSecQ1 = "Favorite drink?";
            string expectedSecQ2 = "Favorite pantone?";
            string expectedSecQ3 = "Favorite thing to do?";
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            //Act 
            var response = pm.GetSecurityQuestions(newlyAddedPasswordReset.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.AreNotEqual(response[0], expectedSecQ1);
            Assert.AreNotEqual(response[1], expectedSecQ2);
            Assert.AreNotEqual(response[2], expectedSecQ3);
        }

        [TestMethod]
        public void CheckSecurityAnswers_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            string secA1 = "Pizza";
            string secA2 = "Cyan";
            string secA3 = "Hiking";
            newUser.SecurityQ1Answer = secA1;
            newUser.SecurityQ2Answer = secA2;
            newUser.SecurityQ3Answer = secA3;
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            List<string> submittedAnswers = new List<string>
            {
                "Pizza",
                "Cyan",
                "Photography"
            };
            //Act
            var response = pm.CheckSecurityAnswers(newlyAddedPasswordReset.ResetToken, submittedAnswers);
            //Assert
            Assert.IsNotNull(response);
            Assert.IsFalse(response);
        }

        [TestMethod]
        public void CheckIfPasswordResetAllowed_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            newlyAddedPasswordReset.AllowPasswordReset = false;
            pm.UpdatePasswordReset(newlyAddedPasswordReset);
            //Act
            var response = pm.CheckIfPasswordResetAllowed(newlyAddedPasswordReset.ResetToken);
            //Assert
            Assert.IsNotNull(response);
            Assert.IsFalse(response);
        }

        [TestMethod]
        public void SendResetEmail_Pass()
        {
            //Arrage
            var email = "winnmoo@gmail.com";
            string url = "kfc-sso.com/resetpassword/";
            int expected = 1;
            //Act
            int actual = pm.SendResetEmail(email, url);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SendResetEmail_Fail()
        {
            //Arrange
            string email = null;
            string url = "kfc-sso.com/resetpassword/";
            int expected = 0;
            //Act
            int actual = pm.SendResetEmail(email, url);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckSecurityAnswersController_Pass()
        {
            //Arrange
            var expected = 1;
            var newUser = tu.CreateUserObject();
            string secA1 = "Pizza";
            string secA2 = "Cyan";
            string secA3 = "Hiking";
            newUser.SecurityQ1Answer = secA1;
            newUser.SecurityQ2Answer = secA2;
            newUser.SecurityQ3Answer = secA3;
            tu.CreateUserInDb(newUser);

            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);

            SecurityAnswersRequest request = new SecurityAnswersRequest();
            request.securityA1 = secA1;
            request.securityA2 = secA2;
            request.securityA3 = secA3;

            //Act
            var actual = pm.CheckSecurityAnswersController(newlyAddedPasswordReset.ResetToken, request);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckSecurityAnswersController_Fail_InvalidToken()
        {
            //Arrange
            var expected = -2;
            var newUser = tu.CreateUserObject();
            string secA1 = "Pizza";
            string secA2 = "Cyan";
            string secA3 = "Hiking";
            newUser.SecurityQ1Answer = secA1;
            newUser.SecurityQ2Answer = secA2;
            newUser.SecurityQ3Answer = secA3;
            tu.CreateUserInDb(newUser);

            var newlyAddedPasswordReset = "asdf";

            SecurityAnswersRequest request = new SecurityAnswersRequest();
            request.securityA1 = secA1;
            request.securityA2 = secA2;
            request.securityA3 = secA3;

            //Act
            var actual = pm.CheckSecurityAnswersController(newlyAddedPasswordReset, request);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckSecurityAnswersController_Fail_WrongAnswers()
        {
            //Arrange
            var expected = -1;
            var newUser = tu.CreateUserObject();
            string secA1 = "Pizza";
            string secA2 = "Cyan";
            string secA3 = "Hiking";
            newUser.SecurityQ1Answer = secA1;
            newUser.SecurityQ2Answer = secA2;
            newUser.SecurityQ3Answer = "photography";
            tu.CreateUserInDb(newUser);

            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);

            SecurityAnswersRequest request = new SecurityAnswersRequest();
            request.securityA1 = secA1;
            request.securityA2 = secA2;
            request.securityA3 = secA3;

            //Act
            var actual = pm.CheckSecurityAnswersController(newlyAddedPasswordReset.ResetToken, request);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ResetPasswordController_Pass()
        {
            //Arrange
            var expected = 1;
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            newlyAddedPasswordReset.AllowPasswordReset = true;
            pm.UpdatePasswordReset(newlyAddedPasswordReset);

            var newPassword = "qweruianvkdasd123";
            //Act
            var actual = pm.ResetPasswordController(newlyAddedPasswordReset.ResetToken, newPassword);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ResetPasswordController_Fail_PasswordLength()
        {
            //Arrange
            var expected = -4;
            var newlyAddedPasswordReset = "asdf";
            var newPassword = "shortpass";
            //Act
            var actual = pm.ResetPasswordController(newlyAddedPasswordReset, newPassword);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ResetPasswordController_Fail_InvalidToken()
        {
            //Arrange
            var expected = -3;
            var newlyAddedPasswordReset = "asdf";
            var newPassword = "qweruianvkdasd123";
            //Act
            var actual = pm.ResetPasswordController(newlyAddedPasswordReset, newPassword);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ResetPasswordController_Fail_ResetNotAllowed()
        {
            //Arrange
            var expected = -2;
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);

            var newPassword = "qweruianvkdasd123";
            //Act
            var actual = pm.ResetPasswordController(newlyAddedPasswordReset.ResetToken, newPassword);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ResetPasswordController_Fail_PasswordPwned()
        {
            //Arrange
            var expected = -1;
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);
            var newlyAddedPasswordReset = pm.CreatePasswordReset(newUser.Id);
            newlyAddedPasswordReset.AllowPasswordReset = true;
            pm.UpdatePasswordReset(newlyAddedPasswordReset);

            var newPassword = "passwordpassword";
            //Act
            var actual = pm.ResetPasswordController(newlyAddedPasswordReset.ResetToken, newPassword);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdatePasswordController_Pass()
        {
            // Arrange
            var expected = 1;
            var newUser = tu.CreateUserObject();

            var oldPassword = "qwertyswag123"; //These three lines of code change the password of the user using the password salt
            var oldPasswordHashed = pm.HashPassword(oldPassword, newUser.PasswordSalt); //so that the password hash is known
            newUser.PasswordHash = oldPasswordHashed;

            tu.CreateUserInDb(newUser);

            var newSession = tu.CreateSessionInDb(newUser);

            UpdatePasswordRequest request = new UpdatePasswordRequest();
            request.sessionToken = newSession.Token;
            request.oldPassword = oldPassword;
            request.newPassword = "123qwertyswag";

            //Act
            var actual = pm.UpdatePasswordController(request);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdatePasswordController_Fail_SamePassword()
        {
            // Arrange
            var expected = -1;
            var newUser = tu.CreateUserObject();
            var oldPassword = "qwertyswag123";
            var oldPasswordHashed = pm.HashPassword(oldPassword, newUser.PasswordSalt);
            newUser.PasswordHash = oldPasswordHashed;
            tu.CreateUserInDb(newUser);

            var newSession = tu.CreateSessionInDb(newUser);

            UpdatePasswordRequest request = new UpdatePasswordRequest();
            request.sessionToken = newSession.Token;
            request.oldPassword = oldPassword;
            request.newPassword = "qwertyswag123";

            //Act
            var actual = pm.UpdatePasswordController(request);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdatePasswordController_Fail_PwnedPassword()
        {
            // Arrange
            var expected = -2;
            var newUser = tu.CreateUserObject();
            var oldPassword = "qwertyswag123";
            var oldPasswordHashed = pm.HashPassword(oldPassword, newUser.PasswordSalt);
            newUser.PasswordHash = oldPasswordHashed;
            tu.CreateUserInDb(newUser);

            var newSession = tu.CreateSessionInDb(newUser);

            UpdatePasswordRequest request = new UpdatePasswordRequest();
            request.sessionToken = newSession.Token;
            request.oldPassword = oldPassword;
            request.newPassword = "passwordpassword";

            //Act
            var actual = pm.UpdatePasswordController(request);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdatePasswordController_Fail_PasswordLength()
        {
            // Arrange
            var expected = -3;
            var newUser = tu.CreateUserObject();
            var oldPassword = "qwertyswag123";
            var oldPasswordHashed = pm.HashPassword(oldPassword, newUser.PasswordSalt);
            newUser.PasswordHash = oldPasswordHashed;
            tu.CreateUserInDb(newUser);

            var newSession = tu.CreateSessionInDb(newUser);

            UpdatePasswordRequest request = new UpdatePasswordRequest();
            request.sessionToken = newSession.Token;
            request.oldPassword = oldPassword;
            request.newPassword = "eucildjeo";

            //Act
            var actual = pm.UpdatePasswordController(request);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdatePasswordController_Fail_InvalidPassword()
        {
            // Arrange
            var expected = -4;
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);

            var newSession = tu.CreateSessionInDb(newUser);

            UpdatePasswordRequest request = new UpdatePasswordRequest();
            request.sessionToken = newSession.Token;
            request.oldPassword = "qwertyswag123";
            request.newPassword = "eucildjeo";

            //Act
            var actual = pm.UpdatePasswordController(request);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdatePasswordController_Fail_InvalidSession()
        {
            // Arrange
            var expected = -5;
            var newUser = tu.CreateUserObject();
            tu.CreateUserInDb(newUser);

            UpdatePasswordRequest request = new UpdatePasswordRequest();
            request.sessionToken = "asdf";
            request.oldPassword = "qwertyswag123";
            request.newPassword = "eucildjeo";

            //Act
            var actual = pm.UpdatePasswordController(request);
            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
