using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;


namespace ServiceLayer.Services
{
    public class ResetService : IResetService
    {
        private ResetRepository _resetRepo;

        public ResetService(DatabaseContext _db)
        {
            _resetRepo = new ResetRepository(_db);
        }

        public PasswordReset CreatePasswordReset(PasswordReset passwordReset)
        {
            return _resetRepo.CreateReset(passwordReset);
        }

        public PasswordReset DeletePasswordReset(string resetToken)
        {
            return _resetRepo.DeleteReset(resetToken);
        }

        public PasswordReset GetPasswordReset(string resetToken)
        {
            return _resetRepo.GetReset(resetToken);
        }

        public PasswordReset UpdatePasswordReset(PasswordReset passwordResetID)
        {
            return _resetRepo.UpdateReset(passwordResetID);
        }

        public bool ExistingReset(string resetToken)
        {
            var result = GetPasswordReset(resetToken);
            if (result != null)
            {
                return true;
            }
            return false;
        }
    }
}
