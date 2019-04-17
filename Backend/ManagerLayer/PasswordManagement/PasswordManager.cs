using DataAccessLayer.Database;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using DataAccessLayer.Models;
using MimeKit;
using System.Data.Entity.Validation;
using DataAccessLayer.Requests;

namespace ManagerLayer.PasswordManagement
{
    public class PasswordManager
    {
        //Variable for how long the token is supposed to be live, in minutes
        private const double TimeToExpire = 5;

        private IResetService _resetService;
        private IPasswordService _passwordService;
        private IEmailService _emailService;
        private ITokenService _tokenService;

        public PasswordManager()
        {
            _resetService = new ResetService();
            _emailService = new EmailService();
            _tokenService = new TokenService();
            _passwordService = new PasswordService();
        }

        private DatabaseContext CreateDbContext()
        {
            return new DatabaseContext();
        }

        //Creates a password reset object
        public PasswordReset CreatePasswordReset(Guid userID)
        {
            string generatedResetToken = _tokenService.GenerateToken();

            DateTime newExpirationTime = DateTime.Now.AddMinutes(TimeToExpire);

            using (var _db = CreateDbContext())
            {
                PasswordReset passwordReset = new PasswordReset
                {
                    ResetToken = generatedResetToken,
                    UserID = userID
                };
                var response = _resetService.CreatePasswordReset(_db, passwordReset);
                try
                {
                    _db.SaveChanges();
                    return response;
                }
                catch (DbEntityValidationException ex)
                {
                    //catch error
                    //detach PasswordReset attempted to be created from the db context - rollback
                    _db.Entry(response).State = System.Data.Entity.EntityState.Detached;
                }
                return null;
            }
        }

        //Deletes password reset object from database
        public int DeletePasswordReset(string resetToken)
        {
            using (var _db = CreateDbContext())
            {
                _resetService.DeletePasswordReset(_db, resetToken);
                return _db.SaveChanges();
            }
        }

        //Gets password reset object from database
        public PasswordReset GetPasswordReset(string resetToken)
        {
            using (var _db = CreateDbContext())
            {
                return _resetService.GetPasswordReset(_db, resetToken);
            }
        }

        //Updates password reset object in database
        public int UpdatePasswordReset(PasswordReset updatedPasswordReset)
        {
            using (var _db = CreateDbContext())
            {
                var response = _resetService.UpdatePasswordReset(_db, updatedPasswordReset);
                try
                {
                    return _db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // catch error
                    // rollback changes
                    _db.Entry(response).CurrentValues.SetValues(_db.Entry(response).OriginalValues);
                    _db.Entry(response).State = System.Data.Entity.EntityState.Unchanged;
                    return 0;
                }
            }
        }

        //Gets the expiration time of the password reset object
        public DateTime GetPasswordResetExpiration(string resetToken)
        {
            var passwordResetRetrieved = GetPasswordReset(resetToken);
            if (passwordResetRetrieved != null)
            {
                return passwordResetRetrieved.ExpirationTime;
            }
            return DateTime.MinValue;
        }

        //Checks the table of password resets to see if the reset token is found
        public bool ExistingResetToken(string resetToken)
        {
            using (var _db = CreateDbContext())
            {
                return _resetService.ExistingReset(_db, resetToken);
            }
        }

        //Gets the number of attempts per password reset object
        public int GetAttemptsPerToken(string resetToken)
        {
            var passwordResetRetrieved = GetPasswordReset(resetToken);
            if (passwordResetRetrieved != null)
            {
                return passwordResetRetrieved.ResetCount;
            }
            return -1;
        }

        //Gets the status of the password reset object
        public bool GetPasswordResetStatus(string resetToken)
        {
            var passwordResetRetrieved = GetPasswordReset(resetToken);
            if (passwordResetRetrieved != null)
            {
                return passwordResetRetrieved.Disabled; 
            }
            return false;
        }

        //Takes the url of the application and appends the reset token 
        public string CreateResetURL(string baseURL, string resetToken)
        {
            string resetControllerURL = baseURL;
            string resetURL = resetControllerURL + resetToken;
            return resetURL;
        }

        //Completely disables the PasswordReset from resetting password
        //by disabling the password reset object, and doesn't allow a password reset
        public void LockPasswordReset(string resetToken)
        {
            var passwordResetRetrieved = GetPasswordReset(resetToken);
            passwordResetRetrieved.Disabled = true;
            passwordResetRetrieved.AllowPasswordReset = false;
            UpdatePasswordReset(passwordResetRetrieved);
        }
        
        public bool CheckPasswordResetValid(string resetToken)
        {
            //See if ResetID exists 
            if (ExistingResetToken(resetToken)) //Checks the DB for the password reset object
            {
                if (GetPasswordResetExpiration(resetToken) > DateTime.Now) //Checks the expiration time on the password reset object and compares it to now
                {                                                          //False if the time now is after the expiration time
                    if (!GetPasswordResetStatus(resetToken)) //Checks to see if the password reset object is disabled
                    {
                        if (GetAttemptsPerToken(resetToken) < 4) //Checks if the token has been used more than 3 times
                        {
                            return true; //return true if token is valid
                        }
                    }
                }
                else
                {
                    LockPasswordReset(resetToken); //Disables the token
                }
            }
            return false; //returns false if any of the checks fails
        }

        //Counts how many times a user has created a new password reset object in the past 24 hours
        public int PasswordResetsMadeInPast24HoursByUser(Guid UserID)
        {
            int numOfResetLinks = 3; //Default value of 3 in case of failed query
            DateTime past24Hours = DateTime.Now.AddDays(-1);
            DateTime currentTime = DateTime.Now.AddMinutes(5);
            using (var _db = CreateDbContext())
            {
                var listOfTokensFrom24Hours = from r in _db.PasswordResets 
                                              where r.ExpirationTime <= currentTime & r.ExpirationTime >= past24Hours & r.UserID == UserID
                                              select r;
                numOfResetLinks = listOfTokensFrom24Hours.Count();
                return numOfResetLinks;
            }
        }

        //Method to create password reset and send to user
        public void SendResetToken(string email, string url)
        {
            using (var _db = CreateDbContext())
            {
                UserService _userService = new UserService(_db);
                if(_userService.ExistingUser(email))
                {
                    Guid userID = _userService.GetUser(email).Id;

                    if (PasswordResetsMadeInPast24HoursByUser(userID) < 3)
                    {
                        PasswordReset newlyCreatedPasswordReset = CreatePasswordReset(userID); //Create a new token
                        string resetToken = newlyCreatedPasswordReset.ResetToken; 
                        string resetLink = CreateResetURL(url, resetToken); //Create the reset URL
                        SendResetEmailUserExists(email, resetLink); //Email the user the reset url
                    }
                    else
                    {
                        SendResetEmailUserExistsTooManyResets(email); //Send email to user if too many attempts have been made
                    }
                }
                else
                {
                    SendResetEmailUserDoesNotExist(email); //Send email to user if not in DB
                }
            }
        }
        
        public bool CheckIsPasswordPwned(string newPasswordToCheck)
        {
            return (_passwordService.CheckPasswordPwned(newPasswordToCheck) > 3);
        }

        //Create a new salt and hash the plaintext password with the new salt
        public string SaltAndHashPassword(string resetToken, string password)
        {
            var retrievedPasswordReset = GetPasswordReset(resetToken);
            var userIDAssociatedWithPasswordReset = retrievedPasswordReset.UserID; //Get the user associated with the reset token
            byte[] salt = _passwordService.GenerateSalt(); //Create new salt

            using (var _db = CreateDbContext())
            {
                var userToUpdate = _db.Users.Find(userIDAssociatedWithPasswordReset); 
                if (userToUpdate != null)
                {
                    userToUpdate.PasswordSalt = salt; //Save the new salt
                    _db.SaveChanges();
                    string hashedPassword = _passwordService.HashPassword(password, salt); //Hash the plaintext password with generated salt
                    return hashedPassword;
                }
            }
            return null;
        }

        //Takes the salt and hashes the plaintext password with the salt
        public string HashPassword(string password, byte[] salt)
        {
            return _passwordService.HashPassword(password, salt);
        }

        //This password update function is for when the user is not logged in and has answered the security questions
        public bool ResetPassword(string resetToken, string newPasswordHash)
        {
            var retrievedPasswordReset = GetPasswordReset(resetToken);
            if(retrievedPasswordReset != null)
            {
                var userIDAssociatedWithPasswordReset = retrievedPasswordReset.UserID;

                using (var _db = CreateDbContext())
                {
                    var userToUpdate = _db.Users.Find(userIDAssociatedWithPasswordReset);
                    if (userToUpdate != null)
                    {
                        userToUpdate.PasswordHash = newPasswordHash;
                        _db.SaveChanges();
                        LockPasswordReset(resetToken);
                        _db.SaveChanges();
                        SendPasswordChange(userToUpdate.Email);
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        //This password update function is for when the user is already logged in and wants to update their password
        public bool UpdatePassword(User userToUpdate, string newPasswordHash)
        {
            using (var _db = CreateDbContext())
            {
                var userRetrieved = _db.Users.Find(userToUpdate.Id);
                var storedHash = userToUpdate.PasswordHash;
                if (storedHash == newPasswordHash)
                {
                    return false;
                }
                else
                {
                    userRetrieved.PasswordHash = newPasswordHash;
                    _db.SaveChanges();
                    return true;
                }
            }
        }

        //Gets security questions from the DB by finding the user associated with the reset token
        public List<string> GetSecurityQuestions(string resetToken)
        {
            List<string> listOfSecurityQuestions = new List<string>();
            var retrievedPasswordReset = GetPasswordReset(resetToken);
            var userID = retrievedPasswordReset.UserID;

            using (var _db = CreateDbContext())
            {
                UserService _userService = new UserService(_db);
                User retrievedUser = _userService.GetUser(userID);
                var securityQ1 = retrievedUser.SecurityQ1;
                var securityQ2 = retrievedUser.SecurityQ2;
                var securityQ3 = retrievedUser.SecurityQ3;
                listOfSecurityQuestions.Add(securityQ1);
                listOfSecurityQuestions.Add(securityQ2);
                listOfSecurityQuestions.Add(securityQ3);
                return listOfSecurityQuestions;
            }
        }

        //Checks the provided security answers against the DB
        public bool CheckSecurityAnswers(string resetToken, List<string> userSubmittedSecurityAnswers)
        {
            List<string> listOfSecurityAnswers = new List<string>();
            var retrievedPasswordReset = GetPasswordReset(resetToken);
            var userID = retrievedPasswordReset.UserID;
            using (var _db = CreateDbContext())
            {
                UserService _userService = new UserService(_db);
                User retrievedUser = _userService.GetUser(userID);
                var securityA1 = retrievedUser.SecurityQ1Answer;
                var securityA2 = retrievedUser.SecurityQ2Answer;
                var securityA3 = retrievedUser.SecurityQ3Answer;
                listOfSecurityAnswers.Add(securityA1);
                listOfSecurityAnswers.Add(securityA2);
                listOfSecurityAnswers.Add(securityA3);
                for (int i = 0; i < listOfSecurityAnswers.Count; i++)
                {
                    //If the answers provided don't match the answers in the DB, the number of attempts to reset the password with that resetID is incremented
                    if (listOfSecurityAnswers[i] != userSubmittedSecurityAnswers[i])
                    {
                        retrievedPasswordReset.ResetCount = retrievedPasswordReset.ResetCount + 1;
                        UpdatePasswordReset(retrievedPasswordReset);
                        if (GetPasswordReset(retrievedPasswordReset.ResetToken).ResetCount > 4)
                        {
                            retrievedPasswordReset.Disabled = true;
                            UpdatePasswordReset(retrievedPasswordReset);
                        }
                        return false; 
                    }
                }
                retrievedPasswordReset.AllowPasswordReset = true; //These lines get called if the answers are correct
                UpdatePasswordReset(retrievedPasswordReset);
                return true;
            }
        }

        //Checks to see if 'AllowPasswordReset' column in password reset object is true
        //Default is false, set to true when the security answers have been answered successfully
        public bool CheckIfPasswordResetAllowed(string resetToken)
        {
            using (var _db = CreateDbContext())
            {
                var resetTokenRetrieved = _resetService.GetPasswordReset(_db, resetToken);
                return resetTokenRetrieved.AllowPasswordReset;
            }
        }


        #region Controller Methods
        //Function to start the reset password process
        public int SendResetEmail(string emailAddress, string URL)
        {
            if(emailAddress != null)
            {
                try
                {
                    SendResetToken(emailAddress, URL);
                    return 1; //1 for 200 response
                }
                catch
                {
                    return -1; //-1 for 503 error
                }
            }
            else
            {
                return 0; // 0 for Unauthorized
            }
        }

        public int CheckSecurityAnswersController(string resetToken, SecurityAnswersRequest request)
        {
            if (CheckPasswordResetValid(resetToken))
            {
                List<string> userSubmittedSecurityAnswers = new List<string>
                {
                    request.securityA1,
                    request.securityA2,
                    request.securityA3
                };
                if (CheckSecurityAnswers(resetToken, userSubmittedSecurityAnswers))
                {
                    return 1; //Ok
                }
                return -1; //Bad Request
            }
            return -2; //Unauthorized
        }

        //Function to get the security answers 
        public int ResetPasswordController(string resetToken, string newPassword)
        {
            if(newPassword != null && newPassword.Length < 2001 && newPassword.Length > 11)
            {
                if (CheckPasswordResetValid(resetToken))
                {
                    if (CheckIfPasswordResetAllowed(resetToken))
                    {
                        if (!CheckIsPasswordPwned(newPassword))
                        {
                            string newPasswordHashed = SaltAndHashPassword(resetToken, newPassword);
                            ResetPassword(resetToken, newPasswordHashed);
                            return 1; //1 for 200
                        }
                        return -1; // -1 for bad request
                    }
                    return -2; // -2 for unauthorized
                }
                return -3; // -3 for unauthorized
            }
            return -4; //-4 for BadRequest
        }

        //Controller method to update the password
        public int UpdatePasswordController(UpdatePasswordRequest request)
        {
            using (var _db = CreateDbContext())
            {
                var _userService = new UserService(_db);
                var _sessionService = new SessionService(_db);
                AuthorizationManager am = new AuthorizationManager(_db);
                if (am.ValidateAndUpdateSession(request.sessionToken) != null)
                {
                    var session = _sessionService.GetSession(request.sessionToken);
                    var user = _userService.GetUser(session.UserId);
                    string oldPasswordHashed = HashPassword(request.oldPassword, user.PasswordSalt);
                    if (oldPasswordHashed == user.PasswordHash)
                    {
                        if (request.newPassword.Length >= 12 && request.newPassword.Length <= 2000)
                        {
                            if (!CheckIsPasswordPwned(request.newPassword))
                            {
                                string newPasswordHashed = HashPassword(request.newPassword, user.PasswordSalt);
                                if (UpdatePassword(user, newPasswordHashed))
                                {
                                    return 1; //OK
                                }
                                return -1; //Bad Request, new password is same as old password
                            }
                            return -2; //Bad Request, pwned password
                        }
                        return -3; //Bad Request, password length
                    }
                    return -4; //Unauthorized, inputted password not the same as old password
                }
                return -5; //Bad Request, invalid session
            }
        }
        #endregion 


        #region Email Methods
        //Function to create the email is user exists 
        public void SendResetEmailUserExists(string receiverEmail, string resetURL)
        {
            string resetPasswordSubjectString = "KFC SSO Reset Password";
            string userFullName = receiverEmail;
            string template = "Hi, \r\n" +
                                             "You recently requested to reset your password for your KFC account, click the link below to reset it.\r\n" +
                                             "The URL is only valid for the next 5 minutes\r\n {0}\r\n\r\n" +
                                             "If you did not request to reset your password, please contact us by responding to this email.\r\n\r\n" +
                                             "Thanks, KFC Team";
            string data = resetURL;
            string resetPasswordBodyString = string.Format(template, data);

            //Create the message that will be sent
            MimeMessage emailToSend = _emailService.CreateEmailPlainBody(userFullName, receiverEmail, resetPasswordSubjectString, resetPasswordBodyString);
            //Send the email with the message
            _emailService.SendEmail(emailToSend);
        }

        //Function to create the email is user exists, but has too many reset links
        public void SendResetEmailUserExistsTooManyResets(string receiverEmail)
        {
            string resetPasswordSubjectString = "KFC SSO Reset Password";
            string userFullName = receiverEmail;
            string resetPasswordBodyString = "Hi, \r\n" +
                                             "You recently requested to reset your password for your KFC account, however 3 resets have been attempted within the past 24 hours.\r\n" +
                                             "Please wait 24 hours until you attempt to reset your password\r\n\r\n" +
                                             "If you did not request to reset your password, please contact us by responding to this email.\r\n\r\n" +
                                             "Thanks, KFC Team";

            //Create the message that will be sent
            MimeMessage emailToSend = _emailService.CreateEmailPlainBody(userFullName, receiverEmail, resetPasswordSubjectString, resetPasswordBodyString);
            //Send the email with the message
            _emailService.SendEmail(emailToSend);
        }

        //Function to create the email is user doesn't exist
        public void SendResetEmailUserDoesNotExist(string receiverEmail)
        {
            string resetPasswordSubjectString = "KFC SSO Reset Password";
            string userFullName = "Unknown";
            string resetPasswordUserDoesNotExistEmailBody = "Hello, \r\n" +
                              "You (or someone else) entered this email address when trying to reset the password of a KFC account.\r\n" +
                              "However, this email address is not on our database of registered users and therefore the attempt to reset the password has failed.\r\n" +
                              "If you have a KFC account and were expecting this email, please try again using the email address you gave when opening your account." +
                              "If you do not have a KFC account, please ignore this email.\r\n" +
                              "For more information about KFC, please visit www.kfc.com/faq \r\n\r\n" +
                              "Best Regards, KFC Team";

            //Create the message that will be sent
            MimeMessage emailToSend = _emailService.CreateEmailPlainBody(userFullName, receiverEmail, resetPasswordSubjectString, resetPasswordUserDoesNotExistEmailBody);
            //Send the email with the message
            _emailService.SendEmail(emailToSend);
        }

        //Function to create the email if the password was changed
        public void SendPasswordChange(string receiverEmail)
        {
            //Need SQL Query to get info about user from DB
            string resetPasswordSubjectString = "KFC SSO Reset Password";
            string userFullName = receiverEmail;
            string changedPasswordBody = "Hi, \r\n" +
                                             "You have changed your password on KFC SSO.\r\n" +
                                             "If you did not change your password, please contact us by responding to this email.\r\n\r\n" +
                                             "Thanks, KFC Team";
            //Create the email service object
            EmailService es = new EmailService();
            //Create the message that will be sent
            MimeMessage emailToSend = _emailService.CreateEmailPlainBody(userFullName, receiverEmail, resetPasswordSubjectString, changedPasswordBody);
            //Send the email with the message
            _emailService.SendEmail(emailToSend);
        }
        #endregion

    }
}
