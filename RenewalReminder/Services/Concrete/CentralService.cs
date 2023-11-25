using KvsProject.Data;
using KvsProject.Domain;
using KvsProject.Domain.Enums;
using KvsProject.Services.Abstract;
using KvsProjectr.Domain.Exceptions;

namespace KvsProject.Services.Concrete
{
    public class CentralService : ServiceBase, ICentralService
    {
        private readonly IRepository<Central> _repositoryCentral;
        private readonly IRepository<MarketPermit> _reporsitoryMarketPermit;

        private readonly IRepository<Guest> _repositoryGuest;

        public CentralService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _repositoryCentral = _serviceProvider.GetService<IRepository<Central>>();
            _repositoryGuest = _serviceProvider.GetService<IRepository<Guest>>();
            _reporsitoryMarketPermit = _serviceProvider.GetService<IRepository<MarketPermit>>();
        }

        public async Task<Result<Central>> SaveCentral(Central entity)
        {
            var isTransactional = _unitOfWork.IsTransactional();
            try
            {
                if (entity.Id == 0)
                {
                    var validationResult = _validation.Validate(entity);
                    if (validationResult.HasError)
                    {
                        return new Result<Central>(validationResult);
                    }
                }


                var oldEntity = default(Central);
                if (entity.Id > 0)
                {
                    oldEntity = await _repositoryCentral.Get(a => a.Id == entity.Id);
                    if (oldEntity == null)
                    {
                        throw new BusException("Güncellenecek kayıt bulunamadı.");
                    }
                    entity.CreateDate = oldEntity.CreateDate;


                    entity.StudentId = oldEntity.StudentId;

                    if (entity.CheckInTime == null)
                    {
                        entity.ElapsedTime = 0;
                    }


                    if (entity.CheckInTime != null && oldEntity.CheckOutTime != null)
                    {
                        TimeSpan elapsedTime = (TimeSpan)(entity.CheckInTime - oldEntity.CheckOutTime);
                        entity.ElapsedTime = (int)elapsedTime.TotalMinutes;
                    }
                }

                if (entity.Id > 0)
                {
                    await _repositoryCentral.Update(entity);
                }
                else
                {
                    entity.CheckOutTime = DateTime.Now;
                    await _repositoryCentral.Add(entity);
                }

                return new Result<Central>() { Data = entity };
            }
            catch (Exception ex)
            {
                if (!isTransactional)
                {
                    return new Result<Central>(ex.Message);
                }
                throw;
            }
        }

        public async Task<Result<Guest>> SaveGuest(Guest entity)
        {
            var isTransactional = _unitOfWork.IsTransactional();
            try
            {
                var validationResult = _validation.Validate(entity);
                if (validationResult.HasError)
                {
                    return new Result<Guest>(validationResult);
                }

                var oldEntity = default(Guest);
                if (entity.Id > 0)
                {
                    oldEntity = await _repositoryGuest.Get(a => a.Id == entity.Id);
                    if (oldEntity == null)
                    {
                        throw new BusException("Güncellenecek kayıt bulunamadı.");
                    }
                    entity.CreateDate = oldEntity.CreateDate;
                }

                if (entity.Id > 0)
                {
                    await _repositoryGuest.Update(entity);
                }
                else
                {
                    await _repositoryGuest.Add(entity);
                }

                return new Result<Guest>() { Data = entity };
            }
            catch (Exception ex)
            {
                if (!isTransactional)
                {
                    return new Result<Guest>(ex.Message);
                }
                throw;
            }
        }


        public async Task<Result<MarketPermit>> SaveMarketPermit(MarketPermit entity)
        {
            var isTransactional = _unitOfWork.IsTransactional();
            try
            {
                var oldEntity = default(MarketPermit);

                if (entity.Id == 0)
                {
                    var validationResult = _validation.Validate(entity);
                    if (validationResult.HasError)
                    {
                        return new Result<MarketPermit>(validationResult);
                    }
                    oldEntity = await _reporsitoryMarketPermit.Get(a => a.StudentId == entity.StudentId && DateTime.Now.Date == entity.CreateDate.Date);
                    if (oldEntity != null)
                    {
                        throw new BusException("Bu kişi daha önce eklendi, tekrar ekleyemezsiniz!");
                    }
                }

                if (entity.Id > 0)
                {
                    oldEntity = await _reporsitoryMarketPermit.Get(a => a.Id == entity.Id);
                    if (oldEntity == null)
                    {
                        throw new BusException("Güncellenecek kayıt bulunamadı.");
                    }
                    entity.CreateDate = oldEntity.CreateDate;
                    entity.StudentId = oldEntity.StudentId;
                    entity.CheckOutTime = oldEntity.CheckOutTime;
                    if (oldEntity.CheckInTime != null)
                    {
                        entity.CheckInTime = null;
                    }
                    else
                    {
                        entity.CheckInTime = DateTime.Now;
                    }
                }

                if (entity.Id > 0)
                {
                    await _reporsitoryMarketPermit.Update(entity);
                }
                else
                {
                    entity.CheckOutTime = DateTime.Now;
                    await _reporsitoryMarketPermit.Add(entity);
                }

                return new Result<MarketPermit>() { Data = entity };
            }
            catch (Exception ex)
            {
                if (!isTransactional)
                {
                    return new Result<MarketPermit>(ex.Message);
                }
                throw;
            }
        }


    }
}




