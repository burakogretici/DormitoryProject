using RenewalReminder.Domain;
using RenewalReminder.Service;

namespace RenewalReminder.Services.Abstract
{
    public interface IUserService : IServiceBase
    {
        Task<Result<User>> SaveUSer(User entity);
        Task<Result> DeleteUser(int id);
    }
}
