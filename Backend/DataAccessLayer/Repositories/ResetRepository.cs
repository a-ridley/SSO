using System.Linq;
using DataAccessLayer.Models;
using DataAccessLayer.Database;
using System.Data.Entity;

namespace DataAccessLayer.Repositories
{
    public class ResetRepository
    {
        DatabaseContext _db;
        public ResetRepository(DatabaseContext _db)
        {
            this._db = _db;
        }

        public PasswordReset GetReset(string resetToken)
        {
            var returnedResetToken = _db.PasswordResets
                                     .Where(r => r.ResetToken == resetToken)
                                     .FirstOrDefault<PasswordReset>();
            return returnedResetToken;
        }
        
        public PasswordReset CreateReset(PasswordReset newPasswordReset)
        {
            _db.PasswordResets.Add(newPasswordReset);
            return newPasswordReset;
        }

        public PasswordReset UpdateReset(PasswordReset updatedPasswordReset)
        {
            _db.Entry(updatedPasswordReset).State = EntityState.Modified;
            return updatedPasswordReset;
        }

        public PasswordReset DeleteReset(string passwordResetTokenToDelete)
        {
            var PasswordResetToRemove = _db.PasswordResets
                                     .Where(r => r.ResetToken == passwordResetTokenToDelete)
                                     .FirstOrDefault<PasswordReset>();

            if (PasswordResetToRemove == null)
                return null;
            _db.PasswordResets.Remove(PasswordResetToRemove);
            return PasswordResetToRemove;
        }
    }
}
