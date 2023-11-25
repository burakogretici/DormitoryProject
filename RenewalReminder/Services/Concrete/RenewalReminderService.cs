//using RenewalReminder.Data;
//using RenewalReminder.Domain;
//using RenewalReminder.Service;
//using RenewalReminder.Services.Abstract;
//using RenewalReminderr.Domain.Exceptions;

//namespace RenewalReminder.Services.Concrete
//{
//    public class RenewalReminderService : ServiceBase, IRenewalReminderService
//    {
//        private readonly IRepository<Domain.RenewalReminder> _repositoryRenewalReminder;
//        private readonly IRepository<UserRenewalReminder> _repositoryUserRenewalReminder;
//        private readonly IRepository<User> _repositoryUser;

//        public RenewalReminderService(IServiceProvider serviceProvider) : base(serviceProvider)
//        {
//            _repositoryRenewalReminder = _serviceProvider.GetService<IRepository<Domain.RenewalReminder>>();
//            _repositoryUserRenewalReminder = _serviceProvider.GetService<IRepository<UserRenewalReminder>>();
//            _repositoryUser = _serviceProvider.GetService<IRepository<User>>();
//        }

//        public async Task<Result<Domain.RenewalReminder>> SaveRenewalReminder(Domain.RenewalReminder entity, List<UserRenewalReminder> userRenewalReminders)
//        {
//            var isTransactional = _unitOfWork.IsTransactional();
//            try
//            {
//                var validationResult = _validation.Validate(entity);
//                if (validationResult.HasError)
//                {
//                    return new Result<Domain.RenewalReminder>(validationResult);
//                }
//                var validUserRenewalReminders = userRenewalReminders?.Where(a => !a.Deleted).ToList();
//                entity.UserRenewalReminders = null;

//                var oldEntity = default(Domain.RenewalReminder);
//                if (entity.Id > 0)
//                {
//                    oldEntity = await _repositoryRenewalReminder.Get(a => a.Id == entity.Id);
//                    if (oldEntity == null)
//                    {
//                        throw new BusException("Güncellenecek kayıt bulunamadı.");
//                    }
//                    entity.CreateDate = oldEntity.CreateDate;
//                }

//                if (entity.Id > 0)
//                {
//                    await _repositoryRenewalReminder.Update(entity);
//                }
//                else
//                {
//                    await _repositoryRenewalReminder.Add(entity);
//                }

//                if (userRenewalReminders != null)
//                {
//                    if (oldEntity != null)
//                    {
//                        var oldUserReminders = await _repositoryUserRenewalReminder.Query(a => a.RenewalReminderId == entity.Id);
//                        foreach (var item in validUserRenewalReminders)
//                        {
//                            var oldUserReminder = oldUserReminders.FirstOrDefault(a => a.UserId == item.UserId);
//                            if (oldUserReminder == null)
//                            {
//                                item.RenewalReminderId = entity.Id;
//                                var saveResult = await SaveUserRenewalReminder(item);
//                                if (saveResult.HasError)
//                                {
//                                    throw new BusException(saveResult);
//                                }
//                            }

//                        }
//                        foreach (var item in oldUserReminders.Where(a => !validUserRenewalReminders.Any(b => b.UserId == a.UserId)))
//                        {
//                            await DeleteUserRenewalReminder(item);
//                        }
//                    }
//                    else
//                    {
//                        foreach (var item in validUserRenewalReminders)
//                        {
//                            item.RenewalReminderId = entity.Id;
//                            var saveResult = await SaveUserRenewalReminder(item);
//                            if (saveResult.HasError)
//                            {
//                                throw new BusException(saveResult);
//                            }
//                        }
//                    }
//                }

//                return new Result<Domain.RenewalReminder>() { Data = entity };
//            }
//            catch (Exception ex)
//            {
//                if (!isTransactional)
//                {
//                    return new Result<Domain.RenewalReminder>(ex.Message);
//                }
//                throw;
//            }
//        }
//        public async Task<Result> DeleteRenewalReminder(int id)
//        {
//            var isTransactional = _unitOfWork.IsTransactional();
//            try
//            {
//                if (!isTransactional)
//                {
//                    await _unitOfWork.BeginTransaction();
//                }

//                var getQuery = NewQuery<Domain.RenewalReminder>(a => a.Id == id);
//                var entity = await _repositoryRenewalReminder.Get(id);
//                if (entity == null)
//                {
//                    throw new BusException("Silinecek kayıt bulunamadı.");
//                }
//                var userReminders = await _repositoryUserRenewalReminder.Query(a => a.RenewalReminderId == entity.Id);
//                foreach (var item in userReminders)
//                {
//                    await DeleteUserRenewalReminder(item);
//                }

//                await _repositoryRenewalReminder.Delete(entity);

//                if (!isTransactional)
//                {
//                    await _unitOfWork.CommitTransaction();
//                }

//                return new Result();
//            }
//            catch (Exception ex)
//            {
//                if (!isTransactional)
//                {
//                    await _unitOfWork.RollbackTransaction();
//                    return new Result(ex.Message);
//                }
//                throw;
//            }
//        }

//        public async Task<Result<UserRenewalReminder>> SaveUserRenewalReminder(UserRenewalReminder entity)
//        {
//            var isTransactional = _unitOfWork.IsTransactional();
//            try
//            {
//                var validationResult = _validation.Validate(entity);
//                if (validationResult.HasError)
//                {
//                    return new Result<UserRenewalReminder>(validationResult);
//                }

//                var oldEntity = default(UserRenewalReminder);
//                if (entity.Id > 0)
//                {
//                    var getQuery = NewQuery<UserRenewalReminder>(a => a.Id == entity.Id);

//                    oldEntity = await _repositoryUserRenewalReminder.Get(getQuery);
//                    if (oldEntity == null)
//                    {
//                        throw new BusException("Güncellenecek kayıt bulunamadı.");
//                    }
//                    entity.CreateDate = oldEntity.CreateDate;
//                    entity.UserId = oldEntity.UserId;
//                    entity.RenewalReminderId = oldEntity.RenewalReminderId;
//                }

//                if (oldEntity == null)
//                {
//                    var exists = await _repositoryUser.Any(a => a.Id == entity.UserId);
//                    if (!exists)
//                    {
//                        throw new BusException("Kullanıcı bulunamadı.");
//                    }

//                    var renewalQuery = NewQuery<Domain.RenewalReminder>(a => a.Id == entity.RenewalReminderId);


//                    var renewalReminder = await _repositoryRenewalReminder.Get(renewalQuery, a => new Domain.RenewalReminder()
//                    {
//                        Id = a.Id,
//                    });

//                    if (renewalReminder == null)
//                    {
//                        throw new BusException("Hatırlatıcı bulunamadı.");
//                    }

//                    exists = await _repositoryUserRenewalReminder.Any(a => a.UserId == entity.UserId && a.RenewalReminderId == entity.RenewalReminderId);
//                    if (exists)
//                    {
//                        throw new BusException("Kullanıcıya bu hatırlatıcı daha önceden eklenmiş.");
//                    }

//                }

//                if (entity.Id > 0)
//                {
//                    await _repositoryUserRenewalReminder.Update(entity);
//                }
//                else
//                {
//                    await _repositoryUserRenewalReminder.Add(entity);
//                }
//                return new Result<UserRenewalReminder>() { Data = entity };
//            }
//            catch (Exception ex)
//            {
//                if (!isTransactional)
//                {
//                    return new Result<UserRenewalReminder>(ex.Message);
//                }
//                throw;
//            }
//        }

//        public async Task<Result> DeleteUserRenewalReminder(int id)
//        {
//            var isTransactional = _unitOfWork.IsTransactional();
//            try
//            {
//                var getQuery = NewQuery<UserRenewalReminder>(a => a.Id == id);
//                var entity = await _repositoryUserRenewalReminder.Get(getQuery);
//                if (entity == null)
//                {
//                    throw new BusException("Silinecek kayıt bulunamadı.");
//                }

//                await DeleteUserRenewalReminder(entity);

//                return new Result();
//            }
//            catch (Exception ex)
//            {
//                if (!isTransactional)
//                {
//                    return new Result(ex.Message);
//                }

//                throw;
//            }
//        }

//        private async Task DeleteUserRenewalReminder(UserRenewalReminder entity)
//        {
//            await _repositoryUserRenewalReminder.Delete(entity);
//        }
//    }
//}