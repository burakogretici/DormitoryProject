using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using KvsProject.Data;
using KvsProject.Domain;
using KvsProject.Models;
using KvsProject.Services.Abstract;
using KvsProject.Services;

namespace KvsProject.Service
{
    public interface IServiceBase
    {
        Task<Result<T>> Get<T>(Expression<Func<T, bool>> filter, params string[] includes) where T : Entity;
        Task<Result<T>> Get<T>(Expression<Func<T, bool>> filter, Expression<Func<T, T>> select = null, params string[] includes) where T : Entity;

        Task<Result<IEnumerable<T>>> Query<T>(Expression<Func<T, bool>> filter, params string[] includes) where T : Entity;
        Task<Result<IEnumerable<T>>> Query<T>(Expression<Func<T, bool>> filter, Expression<Func<T, T>> select = null, params string[] includes) where T : Entity;
        Task<Result<PagedList<T>>> Query<T>(PagedQuery<T> query) where T : Entity;

        Task<Result<bool>> Any<T>(Expression<Func<T, bool>> filter) where T : Entity;
        Expression<Func<T, bool>> NewQuery<T>(Expression<Func<T, bool>> filter) where T : Entity;
        Task<Result> ExecuteTransaction(Func<Task<Result>> func);
    }
}
