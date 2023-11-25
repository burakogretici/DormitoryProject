using KvsProject.Domain;
using KvsProject.Service;

namespace KvsProject.Services.Abstract
{
    public interface IAuthService : IServiceBase
    {
        Task<Result<User>> Login(string email, string password);
        Task<Result> Logout();
    }
}
