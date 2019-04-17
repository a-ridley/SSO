using System;
using ServiceLayer.Services;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Validation;

namespace UnitTesting
{
    /// <summary>
    /// Tests Application Services
    /// </summary>
    [TestClass]
    public class ApplicationServiceUT
    {
        DatabaseContext _db;
        TestingUtils tu;
        Application newApp;
        

        public ApplicationServiceUT()
        {
            var _db = new DatabaseContext();
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
        public void CreateApplication_Pass_ReturnApp()
        {
            // Arrange
            newApp = tu.CreateApplicationObject();
            var expected = newApp;

            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                // Act
                var response = _applicationService.CreateApplication(newApp);
                _db.SaveChanges();

                // Assert
                Assert.IsNotNull(response);
                Assert.AreEqual(response.Id, expected.Id);

                _applicationService.DeleteApplication(response.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void CreateApplication_Fail_ExistingAppShouldReturnNull()
        {
            // Arrange
            newApp = tu.CreateApplicationObject();
            var expected = newApp;

            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                // Act
                var response = _applicationService.CreateApplication(newApp);
                _db.SaveChanges();

                var actual = _applicationService.CreateApplication(newApp);

                // Assert
                Assert.IsNull(actual);
                Assert.AreNotEqual(expected, actual);

                _applicationService.DeleteApplication(response.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void CreateApplication_Fail_MissingFieldsShouldThrowException()
        {
            // Arrange
            newApp = tu.CreateApplicationObject();
            newApp.Title = null;
            var expected = newApp;

            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                // Act
                var response = _applicationService.CreateApplication(newApp);
                try
                {
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException)
                {
                    // Catch error
                    // Detach Session attempted to be created from the db context - rollback
                    _db.Entry(response).State = System.Data.Entity.EntityState.Detached;
                }
                var result = _db.Applications.Find(newApp.Id);

                // Assert
                Assert.IsNull(result);
                Assert.IsNotNull(response);
                Assert.AreEqual(expected, response);
                Assert.AreNotEqual(expected, result);
            }
        }

        [TestMethod]
        public void CreateApplication_Fail_NullValuesReturnNullReferenceException()
        {
            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                bool expected = true; ;

                try
                {
                    // Act
                    var result = _applicationService.CreateApplication(null);
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
        public void DeleteApplication_Pass_ReturnApp()
        {
            // Arrange
            newApp = tu.CreateApplicationObject();

            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                // Act
                newApp = _applicationService.CreateApplication(newApp);
                var expected = newApp;

                _db.SaveChanges();

                var response = _applicationService.DeleteApplication(newApp.Id);
                _db.SaveChanges();
                var result = _db.Applications.Find(expected.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNull(result);
                Assert.AreEqual(response.Id, expected.Id);
            }
        }

        [TestMethod]
        public void DeleteApplication_Fail_NonExistingAppShouldReturnNull()
        {
            // Arrange
            Guid nonExistingId = Guid.NewGuid();

            var expected = nonExistingId;

            using (var _db = new DatabaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                // Act
                var response = _applicationService.DeleteApplication(nonExistingId);
                _db.SaveChanges();
                var result = _db.Applications.Find(expected);

                // Assert
                Assert.IsNull(response);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void UpdateApplication_Pass_ReturnApp()
        {
            // Arrange
            newApp = tu.CreateApplicationObject();
            var expected = newApp.Title;

            // Act
            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                newApp = _applicationService.CreateApplication(newApp);
                _db.SaveChanges();

                newApp.Title = "A new title";
                var response = _applicationService.UpdateApplication(newApp);
                _db.SaveChanges();

                var result = _db.Applications.Find(newApp.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.Id, newApp.Id);
                Assert.AreNotEqual(expected, result.Title);

                _applicationService.DeleteApplication(newApp.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void UpdateApplication_Fail_NonExistingAppShouldThrowException()
        {
            // Arrange
            newApp = tu.CreateApplicationObject();
            var expected = newApp;

            // Act
            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                var response = _applicationService.UpdateApplication(newApp);
                try
                {
                    _db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    _db.Entry(newApp).State = System.Data.Entity.EntityState.Detached;
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException)
                {
                    _db.Entry(newApp).State = System.Data.Entity.EntityState.Detached;
                }
                var result = _db.Applications.Find(expected.Id);

                // Assert
                Assert.IsNull(response);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void UpdateApplication_Fail_NullValuesReturnNullReferenceException()
        {
            bool expected = false;

            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                try
                {
                    // Act
                    var result = _applicationService.UpdateApplication(null);
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
        public void GetApplicationById_Pass_ReturnApp()
        {
            // Arrange
            newApp = tu.CreateApplicationObject();
            var expected = newApp;

            // Act
            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                newApp = _applicationService.CreateApplication(newApp);
                _db.SaveChanges();
                var result = _applicationService.GetApplication(newApp.Id);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expected.Id, result.Id);

                _applicationService.DeleteApplication(newApp.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void GetApplicationById_Fail_NonExistingAppShouldReturnNull()
        {
            // Arrange
            Guid nonExistingApp = Guid.NewGuid();
            Application expected = null;

            // Act
            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                var result = _applicationService.GetApplication(nonExistingApp);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void GetApplicationByTitleEmail_Pass_ReturnApp()
        {
            // Arrange
            newApp = tu.CreateApplicationObject();
            var expected = newApp;

            // Act
            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                newApp = _applicationService.CreateApplication(newApp);
                _db.SaveChanges();
                var result = _applicationService.GetApplication(newApp.Title, newApp.Email);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expected.Title, result.Title);
                Assert.AreEqual(expected.Email, result.Email);

                _applicationService.DeleteApplication(newApp.Id);
                _db.SaveChanges();
            }
        }

        [TestMethod]
        public void GetApplicationByTitleEmail_Fail_NonExistingAppShouldReturnNull()
        {
            // Arrange
            string nonExistingTitle = "title";
            string nonExistingEmail = "email";
            Application expected = null;

            // Act
            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                var result = _applicationService.GetApplication(nonExistingTitle, nonExistingEmail);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void GetApplicationByTitleEmail_Fail_NullValuesReturnNull()
        {
            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                // Act
                var result = _applicationService.GetApplication(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void GetAllApplications_Pass_ReturnAllApps()
        {
            using (var _db = tu.CreateDataBaseContext())
            {
                IApplicationService _applicationService = new ApplicationService(_db); ;

                // Arrange
                var expected = true;
                var actual = false;
                var applications = _applicationService.GetAllApplications();

                // Act
                if (applications != null)
                {
                    actual = true;
                }

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
