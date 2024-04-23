using Jwt.Models;

namespace Jwt.Services;

public interface IResetPasswordService
{
    public Task ForgetPassword(VerificationRequest request);
    public Task ResetPassword(ResetPasswordModel model);
}