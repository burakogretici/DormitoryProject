using RenewalReminder.Data;
using RenewalReminder.Domain;
using RenewalReminder.Domain.Enums;
using RenewalReminder.Services.Abstract;
using RenewalReminderr.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace RenewalReminder.Services.Concrete
{
    public class UserService : ServiceBase, IUserService
    {
        private readonly IRepository<User> _repositoryUser;
        public UserService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _repositoryUser = _serviceProvider.GetService<IRepository<User>>();
        }

        public async Task<Result> DeleteUser(int id)
        {
            var isTransactional = _unitOfWork.IsTransactional();
            try
            {

                if (!isTransactional)
                {
                    await _unitOfWork.BeginTransaction();
                }

                var entity = await _repositoryUser.Get(id);
                if (entity == null)
                {
                    throw new BusException("Silinecek kayıt bulunamadı.");
                }

                await _repositoryUser.Delete(entity);

                if (!isTransactional)
                {
                    await _unitOfWork.CommitTransaction();
                }

                return new Result();
            }
            catch (Exception ex)
            {
                if (!isTransactional)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new Result(ex.Message);
                }
                throw;
            }
        }

        public async Task<Result<User>> SaveUSer(User entity)
        {
            var isTransactional = _unitOfWork.IsTransactional();
            try
            {
                var validationResult = _validation.Validate(entity);
                if (validationResult.HasError)
                {
                    return new Result<User>(validationResult);
                }

                var oldEntity = default(User);
                if (entity.Id > 0)
                {
                    oldEntity = await _repositoryUser.Get(a => a.Id == entity.Id);
                    if (oldEntity == null)
                    {
                        throw new BusException("Güncellenecek kayıt bulunamadı.");
                    }
                    entity.CreateDate = oldEntity.CreateDate;
                }

                if (oldEntity == null || oldEntity.Password != entity.Password)
                {
                    if ((int)GetPasswordScore(entity.Password) < (int)PasswordScore.Medium)
                    {
                        throw new BusException("Şifre en az 8 karakter uzunluğunda, en az 1 küçük, 1 büyük harf ve rakam içermelidir.");
                    }
                    entity.Password = entity.Password.SHA1();
                }

                if (oldEntity == null || oldEntity.Username != entity.Username)
                {
                    var exists = await _repositoryUser.Any(a => a.Username == entity.Username && a.Id != entity.Id);
                    if (exists)
                    {
                        throw new BusException("Bu kullancı adı ile kayıtlı kişi bulunmamaktadır. (" + entity.Username + ")");
                    }
                }
                if (entity.Id > 0)
                {
                    await _repositoryUser.Update(entity);
                }
                else
                {
                    await _repositoryUser.Add(entity);
                }

                return new Result<User>() { Data = entity };
            }
            catch (Exception ex)
            {
                if (!isTransactional)
                {
                    return new Result<User>(ex.Message);
                }
                throw;
            }
        }

        private PasswordScore GetPasswordScore(string password)
        {
            int score = 0;
            if (password.Length < 1)
            {
                score = (int)PasswordScore.Blank;
            }
            if (password.Length < 4)
            {
                score = (int)PasswordScore.VeryWeak;
            }
            else
            {
                if (password.Length >= 8)
                {
                    score++;
                }
                if (password.Length >= 12)
                {
                    score++;
                }
                if (Regex.IsMatch(password, @"[0-9]+(\.[0-9][0-9]?)?", RegexOptions.IgnoreCase)) //number only //"^\d+$" if you need to match more than one digit.
                {
                    score++;
                }
                if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$", RegexOptions.IgnoreCase))  //both, lower and upper case
                {
                    score++;
                }
                if (Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.IgnoreCase)) //^[A-Z]+$
                {
                    score++;
                }
            }

            return (PasswordScore)score;
        }
    }
}
