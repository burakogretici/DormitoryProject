using RenewalReminder.Data;
using RenewalReminder.Domain;
using RenewalReminder.Models;
using RenewalReminder.Service;
using RenewalReminder.Services.Abstract;
using System.Linq.Expressions;

namespace RenewalReminder.Services.Concrete
{
    public class ServiceBase : IServiceBase
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IValidationService _validation;
        protected readonly IUserAccessor _userAccessor;

        public ServiceBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
            _validation = _serviceProvider.GetService<IValidationService>();
            _userAccessor = _serviceProvider.GetService<IUserAccessor>();
        }

        public async Task<Result<T>> Get<T>(Expression<Func<T, bool>> filter, params string[] includes) where T : Entity
        {
            return await Get<T>(filter, null, includes);
        }

        public async Task<Result<T>> Get<T>(Expression<Func<T, bool>> filter, Expression<Func<T, T>> select = null, params string[] includes) where T : Entity
        {
            try
            {
                var repository = _serviceProvider.GetService<IRepository<T>>();
                var data = await repository.Get(filter, select, includes);
                return new Result<T>() { Data = data };
            }
            catch (Exception ex)
            {
                return new Result<T>(ex.Message);
            }
        }

        public async Task<Result<IEnumerable<T>>> Query<T>(Expression<Func<T, bool>> filter, params string[] includes) where T : Entity
        {
            return await Query<T>(filter, null, includes);
        }

        public async Task<Result<IEnumerable<T>>> Query<T>(Expression<Func<T, bool>> filter, Expression<Func<T, T>> select = null, params string[] includes) where T : Entity
        {
            try
            {
                var repository = _serviceProvider.GetService<IRepository<T>>();
                var data = await repository.Query(filter, select, includes);
                return new Result<IEnumerable<T>>() { Data = data };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<T>>(ex.Message);
            }
        }

        public async Task<Result<PagedList<T>>> Query<T>(PagedQuery<T> query) where T : Entity
        {
            try
            {
                var repository = _serviceProvider.GetService<IRepository<T>>();
                var data = await repository.Query(query);
                return new Result<PagedList<T>>() { Data = data };
            }
            catch (Exception ex)
            {
                return new Result<PagedList<T>>(ex.Message);
            }
        }

        public async Task<Result<bool>> Any<T>(Expression<Func<T, bool>> filter) where T : Entity
        {
            try
            {
                var repository = _serviceProvider.GetService<IRepository<T>>();
                var data = await repository.Any(filter);
                return new Result<bool>() { Data = data };
            }
            catch (Exception ex)
            {
                return new Result<bool>(ex.Message);
            }
        }

        public Expression<Func<T, bool>> NewQuery<T>(Expression<Func<T, bool>> filter) where T : Entity
        {
            return filter;
        }

        public async Task<Result> ExecuteTransaction(Func<Task<Result>> func)
        {
            try
            {
                await _unitOfWork.BeginTransaction();

                var result = await func.Invoke();
                if (result.HasError)
                {
                    await _unitOfWork.RollbackTransaction();
                }
                else
                {
                    await _unitOfWork.CommitTransaction();
                }

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new Result(ex.Message);
            }
        }

    }
}
