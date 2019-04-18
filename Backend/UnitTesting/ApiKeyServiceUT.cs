using System;
using ServiceLayer.Services;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace UnitTesting
{
    /// <summary>
    /// Tests ApiKey Services
    /// </summary>
    [TestClass]
    public class ApiKeyServiceUT
    {
        DatabaseContext _db;
        TestingUtils tu;
        ApiKey newKey;

        public ApiKeyServiceUT()
        {
            _db = new DatabaseContext();
            tu = new TestingUtils();
        }


        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CreateKey_Pass_ReturnKey()
        {
            // Arrange
            newKey = tu.CreateApiKeyObject();
            var expected = newKey;

            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);
                IApplicationService _applicationService = new ApplicationService(_db);

                // Act
                var response = _apiKeyService.CreateKey(newKey);
                _db.SaveChanges();

                // Assert
                Assert.IsNotNull(response);
                Assert.AreEqual(response.Id, expected.Id);

                _applicationService.DeleteApplication(newKey.ApplicationId);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void CreateKey_Fail_ExistingKeyShouldReturnNull()
        {
            // Arrange
            newKey = tu.CreateApiKeyObject();
            var expected = newKey;

            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);
                IApplicationService _applicationService = new ApplicationService(_db);

                // Act
                var response = _apiKeyService.CreateKey(newKey);
                _db.SaveChanges();

                var actual = _apiKeyService.CreateKey(newKey);

                // Assert
                Assert.AreNotEqual(expected, actual);
                Assert.IsNotNull(response);
                Assert.IsNull(actual);

                _applicationService.DeleteApplication(newKey.ApplicationId);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void CreateKey_Fail_NullValuesShouldReturnNullReferenceException()
        {
            bool expected = true;

            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);

                try
                {
                    // Act
                    var result = _apiKeyService.CreateKey(null);
                }
                catch (NullReferenceException)
                {
                    expected = false;
                }

                // Assert
                Assert.IsFalse(expected);
            }
        }

        [TestMethod]
        public void DeleteKey_Pass_ReturnKey()
        {
            // Arrange
            newKey = tu.CreateApiKeyObject();

            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);


                // Act
                newKey = _apiKeyService.CreateKey(newKey);
                var expected = newKey;

                _db.SaveChanges();

                var response = _apiKeyService.DeleteKey(newKey.Id);
                _db.SaveChanges();
                var result = _db.Keys.Find(expected.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNull(result);
                Assert.AreEqual(response.Id, expected.Id);
            }
        }

        [TestMethod]
        public void DeleteKey_Fail_NonExistingKeyShouldReturnNull()
        {
            // Arrange
            Guid nonExistingId = Guid.NewGuid();

            var expected = nonExistingId;

            using (_db = new DatabaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);

                // Act
                var response = _apiKeyService.DeleteKey(nonExistingId);
                _db.SaveChanges();
                var result = _db.Keys.Find(expected);

                // Assert
                Assert.IsNull(response);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void UpdateKey_Pass_ReturnKey()
        {
            // Arrange
            newKey = tu.CreateApiKeyObject();
            var expected = newKey.Key;

            // Act
            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);

                newKey = _apiKeyService.CreateKey(newKey);
                _db.SaveChanges();

                newKey.Key = "A new Key";
                var response = _apiKeyService.UpdateKey(newKey);
                _db.SaveChanges();

                var result = _db.Keys.Find(newKey.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.Id, newKey.Id);
                Assert.AreNotEqual(expected, result.Key);

                _apiKeyService.DeleteKey(newKey.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void UpdateKey_Fail_NonExistingKeyShouldThrowException()
        {
            // Arrange
            newKey = tu.CreateApiKeyObject();
            var expected = newKey;

            // Act
            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);

                var response = _apiKeyService.UpdateKey(newKey);
                try
                {
                    _db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    _db.Entry(newKey).State = System.Data.Entity.EntityState.Detached;
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException)
                {
                    _db.Entry(newKey).State = System.Data.Entity.EntityState.Detached;
                }
                var result = _db.Keys.Find(expected.Id);

                // Assert
                Assert.IsNull(response);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void UpdateKey_Fail_NullValuesShouldReturnNull()
        {
            bool expected = true;

            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);

                try
                {
                    // Act
                    var result = _apiKeyService.UpdateKey(null);
                }
                catch (NullReferenceException)
                {
                    expected = false;
                }

                // Assert
                Assert.IsFalse(expected);
            }
        }

        [TestMethod]
        public void GetKeyById_Pass_ReturnKey()
        {
            // Arrange
            newKey = tu.CreateApiKeyObject();
            var expected = newKey;

            // Act
            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);

                newKey = _apiKeyService.CreateKey(newKey);
                _db.SaveChanges();
                var result = _apiKeyService.GetKey(newKey.Id);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expected.Id, result.Id);

                _apiKeyService.DeleteKey(newKey.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void GetApiKeyById_Fail_NonExistingKeyShouldReturnNull()
        {
            // Arrange
            Guid nonExistingKey = Guid.NewGuid();
            ApiKey expected = null;

            // Act
            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);

                var result = _apiKeyService.GetKey(nonExistingKey);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void GetApiKeyByKey_Pass_ReturnKey()
        {
            // Arrange
            newKey = tu.CreateApiKeyObject();
            var expected = newKey;

            // Act
            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);

                newKey = _apiKeyService.CreateKey(newKey);
                _db.SaveChanges();
                var result = _apiKeyService.GetKey(newKey.Key);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expected.Key, result.Key);

                _apiKeyService.DeleteKey(newKey.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void GetApiKeyByKey_Fail_NonExistingKeyShouldReturnNull()
        {
            // Arrange
            string nonExistingKey = "key";
            ApiKey expected = null;

            // Act
            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);

                var result = _apiKeyService.GetKey(nonExistingKey);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void GetApiKeyByKey_Fail_NullValuesShouldReturnNull()
        {
            using (_db = tu.CreateDataBaseContext())
            {
                IApiKeyService _apiKeyService = new ApiKeyService(_db);

                // Act
                var result = _apiKeyService.GetKey(null);

                // Assert
                Assert.IsNull(result);
            }
        }
    }
}
