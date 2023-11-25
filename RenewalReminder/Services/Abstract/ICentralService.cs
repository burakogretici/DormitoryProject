using KvsProject.Domain;
using KvsProject.Service;

namespace KvsProject.Services.Abstract
{
    public interface ICentralService : IServiceBase
    {
        Task<Result<Central>> SaveCentral(Central entity);
        Task<Result<Guest>> SaveGuest(Guest entity);
        Task<Result<MarketPermit>> SaveMarketPermit(MarketPermit entity);

    }
}
