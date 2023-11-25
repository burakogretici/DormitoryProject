using RenewalReminder.Domain;
using RenewalReminder.Service;

namespace RenewalReminder.Services.Abstract
{
    public interface IAuthService : IServiceBase
    {
        Task<Result<User>> Login(string email, string password);
        Task<Result> Logout();
    }
}
