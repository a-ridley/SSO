using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ManagerLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class AuthorizationManagerUT
    {
        TestingUtils _tu;
        DatabaseContext _db;

        public AuthorizationManagerUT()
        {
            _tu = new TestingUtils();
        }

        [TestMethod]
        public void GenerateSession()
        {
            AuthorizationManager _am = new AuthorizationManager(null);
            string sessionToken1 = _am.GenerateSessionToken();
            string sessionToken2 = _am.GenerateSessionToken();

            Assert.AreEqual(sessionToken1.Length, 64);
            Assert.AreNotEqual(sessionToken1, sessionToken2);
        }

        [TestMethod]
        public void Create_And_Validate_Token()
        {
            // Arrange
            User user = _tu.CreateUserInDb();

            // Act
            using (var _db = new DatabaseContext())
            {
                AuthorizationManager _am = new AuthorizationManager(_db);
                Session session = _am.CreateSession(user);

                _db.SaveChanges();

                Session validatedSession = _am.ValidateAndUpdateSession(session.Token);

                // Assert 
                Assert.IsNotNull(validatedSession);
                Assert.AreEqual(session.Token, validatedSession.Token);
                Assert.AreEqual(session.Id, validatedSession.Id);
            }
        }

        [TestMethod]
        public void Validate_Invalid_Token()
        {
            using (var _db = new DatabaseContext())
            {
                AuthorizationManager _am = new AuthorizationManager(_db);
                Session validatedSession = _am.ValidateAndUpdateSession("invalidToken");

                Assert.IsNull(validatedSession);
            }
        }

        [TestMethod]
        public void Create_And_Delete_Token()
        {
            // Arrange
            User user = _tu.CreateUserInDb();
            Session session = _tu.CreateSessionInDb(user);

            // Act
            using (var _db = new DatabaseContext())
            {
                AuthorizationManager _am = new AuthorizationManager(_db);
                Session deletedSession = _am.DeleteSession(session.Token);

                _db.SaveChanges();

                Session validatedSession = _am.ValidateAndUpdateSession(session.Token);

                // Assert 
                Assert.IsNotNull(deletedSession);
                Assert.AreEqual(session.Token, deletedSession.Token);
                Assert.AreEqual(session.Id, deletedSession.Id);
                
            }
        }
    }
}
