using System;
using System.Data.Entity.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using ManagerLayer;

namespace UnitTesting
{
	[TestClass]
	public class SessionTimeOutUT
	{
		[TestMethod]
		public void Session_Time_Out_Success()
		{
			//Arrange
			DatabaseContext _db = new DatabaseContext();
			TestingUtils tu = new TestingUtils();
			SessionService ss = new SessionService(_db);
			AuthorizationManager _am;
			User newUser1 = tu.CreateUserObject();
			User newUser2 = tu.CreateUserObject();
			Session ValidSession;
			Session ExpiredSession;
			//Act
			_am = new AuthorizationManager(_db);
			newUser1 = tu.CreateUserInDb();
			newUser2 = tu.CreateUserInDb();
			ValidSession = tu.CreateSessionInDb(newUser1);
			ExpiredSession = tu.CreateExpiredSessionInDb(newUser2);
			//Assert
			Assert.IsNull(_am.ValidateAndUpdateSession(ExpiredSession.Token));
		}
		[TestMethod]
		public void Session_Time_Out_Deletion_Success()
		{
			//Arrange
			DatabaseContext _db = new DatabaseContext();
			TestingUtils tu = new TestingUtils();
			SessionService ss = new SessionService(_db);
			AuthorizationManager _am;
			User newUser3 = tu.CreateUserObject();
			User newUser4 = tu.CreateUserObject();
			Session ValidSession;
			Session ExpiredSession;
			//Act
			_am = new AuthorizationManager(_db);
			newUser3 = tu.CreateUserInDb();
			newUser4 = tu.CreateUserInDb();
			ValidSession = tu.CreateSessionInDb(newUser3);
			ExpiredSession = tu.CreateExpiredSessionInDb(newUser4);
			//Assert
			Session DeletedSession = _am.ValidateAndUpdateSession(ExpiredSession.Token);
			Assert.IsNull(ss.GetSession(ExpiredSession.Token));
		}
		[TestMethod]
		public void Sesstion_Time_Out_Valid_Session_Success()
		{
			//Arrange
			DatabaseContext _db = new DatabaseContext();
			TestingUtils tu = new TestingUtils();
			SessionService ss = new SessionService(_db);
			AuthorizationManager _am;
			User newUser5 = tu.CreateUserObject();
			Session ValidSession;
			//Act
			_am = new AuthorizationManager(_db);
			newUser5 = tu.CreateUserInDb();
			ValidSession = tu.CreateSessionInDb(newUser5);
			Session TestValid = _am.ValidateAndUpdateSession(ValidSession.Token);
			//Assert
			Assert.AreEqual(TestValid.Token, ValidSession.Token);
		}
	}
}
