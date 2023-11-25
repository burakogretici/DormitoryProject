//using KvsProject.Data;
//using KvsProject.Domain;
//using KvsProject.Service;
//using KvsProject.Services.Abstract;
//using KvsProjectr.Domain.Exceptions;

//namespace KvsProject.Services.Concrete
//{
//    public class KvsProjectService : ServiceBase, IKvsProjectService
//    {
//        private readonly IRepository<Domain.KvsProject> _repositoryKvsProject;
//        private readonly IRepository<UserKvsProject> _repositoryUserKvsProject;
//        private readonly IRepository<User> _repositoryUser;

//        public KvsProjectService(IServiceProvider serviceProvider) : base(serviceProvider)
//        {
//            _repositoryKvsProject = _serviceProvider.GetService<IRepository<Domain.KvsProject>>();
//            _repositoryUserKvsProject = _serviceProvider.GetService<IRepository<UserKvsProject>>();
//            _repositoryUser = _serviceProvider.GetService<IRepository<User>>();
//        }

//        public async Task<Result<Domain.KvsProject>> SaveKvsProject(Domain.KvsProject entity, List<UserKvsProject> userKvsProjects)
//        {
//            var isTransactional = _unitOfWork.IsTransactional();
//            try
//            {
//                var validationResult = _validation.Validate(entity);
//                if (validationResult.HasError)
//                {
//                    return new Result<Domain.KvsProject>(validationResult);
//                }
//                var validUserKvsProjects = userKvsProjects?.Where(a => !a.Deleted).ToList();
//                entity.UserKvsProjects = null;

//                var oldEntity = default(Domain.KvsProject);
//                if (entity.Id > 0)
//                {
//                    oldEntity = await _repositoryKvsProject.Get(a => a.Id == entity.Id);
//                    if (oldEntity == null)
//                    {
//                        throw new BusException("Güncellenecek kayıt bulunamadı.");
//                    }
//                    entity.CreateDate = oldEntity.CreateDate;
//                }

//                if (entity.Id > 0)
//                {
//                    await _repositoryKvsProject.Update(entity);
//                }
//                else
//                {
//                    await _repositoryKvsProject.Add(entity);
//                }

//                if (userKvsProjects != null)
//                {
//                    if (oldEntity != null)
//                    {
//                        var oldUserReminders = await _repositoryUserKvsProject.Query(a => a.KvsProjectId == entity.Id);
//                        foreach (var item in validUserKvsProjects)
//                        {
//                            var oldUserReminder = oldUserReminders.FirstOrDefault(a => a.UserId == item.UserId);
//                            if (oldUserReminder == null)
//                            {
//                                item.KvsProjectId = entity.Id;
//                                var saveResult = await SaveUserKvsProject(item);
//                                if (saveResult.HasError)
//                                {
//                                    throw new BusException(saveResult);
//                                }
//                            }

//                        }
//                        foreach (var item in oldUserReminders.Where(a => !validUserKvsProjects.Any(b => b.UserId == a.UserId)))
//                        {
//                            await DeleteUserKvsProject(item);
//                        }
//                    }
//                    else
//                    {
//                        foreach (var item in validUserKvsProjects)
//                        {
//                            item.KvsProjectId = entity.Id;
//                            var saveResult = await SaveUserKvsProject(item);
//                            if (saveResult.HasError)
//                            {
//                                throw new BusException(saveResult);
//                            }
//                        }
//                    }
//                }

//                return new Result<Domain.KvsProject>() { Data = entity };
//            }
//            catch (Exception ex)
//            {
//                if (!isTransactional)
//                {
//                    return new Result<Domain.KvsProject>(ex.Message);
//                }
//                throw;
//            }
//        }
//        public async Task<Result> DeleteKvsProject(int id)
//        {
//            var isTransactional = _unitOfWork.IsTransactional();
//            try
//            {
//                if (!isTransactional)
//                {
//                    await _unitOfWork.BeginTransaction();
//                }

//                var getQuery = NewQuery<Domain.KvsProject>(a => a.Id == id);
//                var entity = await _repositoryKvsProject.Get(id);
//                if (entity == null)
//                {
//                    throw new BusException("Silinecek kayıt bulunamadı.");
//                }
//                var userReminders = await _repositoryUserKvsProject.Query(a => a.KvsProjectId == entity.Id);
//                foreach (var item in userReminders)
//                {
//                    await DeleteUserKvsProject(item);
//                }

//                await _repositoryKvsProject.Delete(entity);

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

//        public async Task<Result<UserKvsProject>> SaveUserKvsProject(UserKvsProject entity)
//        {
//            var isTransactional = _unitOfWork.IsTransactional();
//            try
//            {
//                var validationResult = _validation.Validate(entity);
//                if (validationResult.HasError)
//                {
//                    return new Result<UserKvsProject>(validationResult);
//                }

//                var oldEntity = default(UserKvsProject);
//                if (entity.Id > 0)
//                {
//                    var getQuery = NewQuery<UserKvsProject>(a => a.Id == entity.Id);

//                    oldEntity = await _repositoryUserKvsProject.Get(getQuery);
//                    if (oldEntity == null)
//                    {
//                        throw new BusException("Güncellenecek kayıt bulunamadı.");
//                    }
//                    entity.CreateDate = oldEntity.CreateDate;
//                    entity.UserId = oldEntity.UserId;
//                    entity.KvsProjectId = oldEntity.KvsProjectId;
//                }

//                if (oldEntity == null)
//                {
//                    var exists = await _repositoryUser.Any(a => a.Id == entity.UserId);
//                    if (!exists)
//                    {
//                        throw new BusException("Kullanıcı bulunamadı.");
//                    }

//                    var renewalQuery = NewQuery<Domain.KvsProject>(a => a.Id == entity.KvsProjectId);


//                    var KvsProject = await _repositoryKvsProject.Get(renewalQuery, a => new Domain.KvsProject()
//                    {
//                        Id = a.Id,
//                    });

//                    if (KvsProject == null)
//                    {
//                        throw new BusException("Hatırlatıcı bulunamadı.");
//                    }

//                    exists = await _repositoryUserKvsProject.Any(a => a.UserId == entity.UserId && a.KvsProjectId == entity.KvsProjectId);
//                    if (exists)
//                    {
//                        throw new BusException("Kullanıcıya bu hatırlatıcı daha önceden eklenmiş.");
//                    }

//                }

//                if (entity.Id > 0)
//                {
//                    await _repositoryUserKvsProject.Update(entity);
//                }
//                else
//                {
//                    await _repositoryUserKvsProject.Add(entity);
//                }
//                return new Result<UserKvsProject>() { Data = entity };
//            }
//            catch (Exception ex)
//            {
//                if (!isTransactional)
//                {
//                    return new Result<UserKvsProject>(ex.Message);
//                }
//                throw;
//            }
//        }

//        public async Task<Result> DeleteUserKvsProject(int id)
//        {
//            var isTransactional = _unitOfWork.IsTransactional();
//            try
//            {
//                var getQuery = NewQuery<UserKvsProject>(a => a.Id == id);
//                var entity = await _repositoryUserKvsProject.Get(getQuery);
//                if (entity == null)
//                {
//                    throw new BusException("Silinecek kayıt bulunamadı.");
//                }

//                await DeleteUserKvsProject(entity);

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

//        private async Task DeleteUserKvsProject(UserKvsProject entity)
//        {
//            await _repositoryUserKvsProject.Delete(entity);
//        }
//    }
//}