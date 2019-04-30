using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;

namespace UnitTesting
{
    [TestClass]
    public class ResetPasswordUT
    {
        TestingUtils tu;
        PasswordReset newPasswordReset;

        public ResetPasswordUT()
        {
            tu = new TestingUtils();
        }
        
        [TestMethod]
        public void CreatePasswordReset_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            newPasswordReset = tu.CreatePasswordResetObject(newUser);
            var expected = newPasswordReset;
            //Act
            using(var _db = tu.CreateDataBaseContext())
            {
                ResetService rs = new ResetService(_db);
                var response = rs.CreatePasswordReset(newPasswordReset);
                _db.SaveChanges();

                //Assert
                Assert.IsNotNull(response);
                Assert.AreEqual(response.ResetToken, expected.ResetToken);
            }
        }

        [TestMethod]
        public void CreatePasswordReset_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            newPasswordReset = tu.CreatePasswordResetObject(newUser);
            var expected = tu.CreatePasswordResetObject(newUser);
            //Act
            using (var _db = tu.CreateDataBaseContext())
            {
                ResetService rs = new ResetService(_db);
                var response = rs.CreatePasswordReset(newPasswordReset);
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception exception)
                {

                }
                //Assert
                Assert.IsNotNull(response);
                Assert.AreNotEqual(response, expected);
            }
        }

        [TestMethod]
        public void GetPasswordReset_Pass()
        {
            //Arrange

            var newUser = tu.CreateUserObject();
            newPasswordReset = tu.CreatePasswordResetObject(newUser);
            var expected = newPasswordReset;
           
            //Act
            using(var _db = tu.CreateDataBaseContext())
            {
                ResetService rs = new ResetService(_db);
                newPasswordReset = rs.CreatePasswordReset(newPasswordReset);
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception exception)
                {

                }
                var result = rs.GetPasswordReset(newPasswordReset.ResetToken);
                //Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(result.ResetToken, expected.ResetToken);
            }

        }

        [TestMethod]
        public void GetPasswordReset_Fail()
        {
            //Arrange
            string NonExistingResetToken = "asdf";
            PasswordReset expected = null;

            //Act
            using (var _db = tu.CreateDataBaseContext())
            {
                ResetService rs = new ResetService(_db);
                var result = rs.GetPasswordReset(NonExistingResetToken);
                //Assert
                Assert.IsNull(result);
                Assert.AreEqual(result, expected);
            }
        }

        [TestMethod]
        public void ExistingPasswordReset_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            newPasswordReset = tu.CreatePasswordResetObject(newUser);
            var expected = true;
            //Act
            using(var _db = tu.CreateDataBaseContext())
            {
                ResetService rs = new ResetService(_db);
                rs.CreatePasswordReset(newPasswordReset);
                _db.SaveChanges();
                var actual = rs.ExistingReset(newPasswordReset.ResetToken);
                //Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(actual, expected);
            }
        }

        [TestMethod]
        public void ExistingPasswordReset_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            newPasswordReset = tu.CreatePasswordResetObject(newUser);
            var expected = true;
            //Act
            using (var _db = tu.CreateDataBaseContext())
            {
                ResetService rs = new ResetService(_db);
                var actual = rs.ExistingReset(newPasswordReset.ResetToken);
                //Assert
                Assert.IsFalse(actual);
                Assert.AreNotEqual(actual, expected);
            }
        }

        [TestMethod]
        public void UpdatePasswordReset_Pass()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            newPasswordReset = tu.CreatePasswordResetObject(newUser);
            var expectedResultExpirationTime = newPasswordReset.ExpirationTime;
            var expectedResult = newPasswordReset;
            //Act
            using(var _db = tu.CreateDataBaseContext())
            {
                ResetService rs = new ResetService(_db);
                newPasswordReset = rs.CreatePasswordReset(newPasswordReset);
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception exception)
                {

                }
                newPasswordReset.ExpirationTime = DateTime.Now.AddYears(5);
                var response = rs.UpdatePasswordReset(newPasswordReset);
                _db.SaveChanges();
                var result = rs.GetPasswordReset(expectedResult.ResetToken);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.ResetToken, expectedResult.ResetToken);
                Assert.AreNotEqual(result.ExpirationTime, expectedResultExpirationTime);
            }
        }

        [TestMethod]
        public void UpdatePasswordReset_Fail()
        {
            //Arrange
            var newUser = tu.CreateUserObject();
            newPasswordReset = tu.CreatePasswordResetObject(newUser);
            PasswordReset expectedResult = newPasswordReset;
            //Act
            using(var _db = tu.CreateDataBaseContext())
            {
                ResetService rs = new ResetService(_db);
                var response = rs.UpdatePasswordReset(newPasswordReset);
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception)
                {
                    // catch error
                    // rollback changes
                    _db.Entry(newPasswordReset).State = System.Data.Entity.EntityState.Detached;
                }
                var result = rs.GetPasswordReset(expectedResult.ResetToken);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void DeletePasswordReset_Pass()
        {
            // Arrange
            var newUser = tu.CreateUserObject();
            newPasswordReset = tu.CreatePasswordResetObject(newUser);

            //Act
            using(var _db = tu.CreateDataBaseContext())
            {
                ResetService rs = new ResetService(_db);
                var expected = rs.CreatePasswordReset(newPasswordReset);

                try
                {
                    _db.SaveChanges();
                }
                catch (Exception exception)
                {

                }

                var response = rs.DeletePasswordReset(expected.ResetToken);
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception exception)
                {

                }

                var result = rs.GetPasswordReset(expected.ResetToken);

                //Assert
                Assert.IsNotNull(response);
                Assert.IsNull(result);
                Assert.AreEqual(response.ResetToken, expected.ResetToken);
            }
        }

        [TestMethod]
        public void DeletePasswordReset_Fail()
        {
            // Arrange
            string nonexistingPasswordResetToken = "asdf";
            var expected = nonexistingPasswordResetToken;

            //Act
            using (var _db = tu.CreateDataBaseContext())
            {
                ResetService rs = new ResetService(_db);
                var response = rs.DeletePasswordReset(expected);
                _db.SaveChanges();

                var result = rs.GetPasswordReset(expected);

                //Assert
                Assert.IsNull(response);
                Assert.IsNull(result);
            }
        }
    }
}
