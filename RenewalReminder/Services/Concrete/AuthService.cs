using RenewalReminder.Data;
using RenewalReminder.Domain;
using RenewalReminder.Services.Abstract;

namespace RenewalReminder.Services.Concrete
{
    public class AuthService : ServiceBase,IAuthService
    {
        private readonly IRepository<User> _repositoryUser;

        public AuthService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _repositoryUser = _serviceProvider.GetRequiredService<IRepository<User>>();
        }

        public async Task<Result<User>> Login(string username, string password)
        {
            try
            {
                var pwd = password.SHA1();
                var user = await _repositoryUser.Get(a => a.Username == username && a.Password == pwd && a.Deleted != true);
                if (user == null)
                {
                    return new Result<User>("Lütfen kullanıcı adı ve şifrenizi kontrol ediniz");
                }
                _userAccessor.User = user;
                return new Result<User>() { Data = user };
            }
            catch (Exception ex)
            {
                return new Result<User>(ex.Message);
            }
        }

        public async Task<Result> Logout()
        {
            try
            {
                _userAccessor.Clear(null);
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(ex.Message);
            }
        }
    }
}
