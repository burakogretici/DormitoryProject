using KvsProject.Domain;
using KvsProject.Service;

namespace KvsProject.Services.Abstract
{
    public interface IUserService : IServiceBase
    {
        Task<Result<User>> SaveUSer(User entity);
        Task<Result> DeleteUser(int id);
    }
}
