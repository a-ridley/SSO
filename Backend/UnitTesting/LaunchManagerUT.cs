using DataAccessLayer.Database;
using ManagerLayer.LaunchManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTesting
{
    [TestClass]
    public class LaunchManagerUT
    {
        TestingUtils _tu;

        public LaunchManagerUT()
        {
            _tu = new TestingUtils();
        }

        [TestMethod]
        public void Sign_Launch_Success()
        {
            // Arrange
            var user = _tu.CreateUserInDb();
            var session = _tu.CreateSessionInDb(user);
            var application = _tu.CreateApplicationInDb();

            // Act
            using (var _db = new DatabaseContext())
            {
                var _lm = new LaunchManager(_db);
                var payload = _lm.SignLaunch(session, application.Id);

                _db.SaveChanges();

                // Assert 
                Assert.IsNotNull(payload);
                Assert.IsNotNull(payload.launchPayload);
                Assert.AreEqual(payload.url, application.LaunchUrl);
            }
        }

        [TestMethod]
        public void Sign_Launch_Invalid_App()
        {
            // Arrange
            var user = _tu.CreateUserInDb();
            var session = _tu.CreateSessionInDb(user);

            using (var _db = new DatabaseContext())
            {
                var _lm = new LaunchManager(_db);
                try
                {
                    _lm.SignLaunch(session, new Guid());

                    throw new Exception("Test failed - Manager did not throw exception.");
                }
                catch(ArgumentException) {} // Expect to throw due to invalid app id
            }
        }
    }
}
