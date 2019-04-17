using DataAccessLayer.Models;

namespace ServiceLayer.Services
{
    public interface IResetService
    {
        PasswordReset CreatePasswordReset(PasswordReset passwordResetID);
        PasswordReset GetPasswordReset(string resetToken);
        PasswordReset UpdatePasswordReset(PasswordReset passwordResetID);
        PasswordReset DeletePasswordReset(string resetToken);
        bool ExistingReset(string resetToken);
    }
}
