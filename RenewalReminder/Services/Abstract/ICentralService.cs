using RenewalReminder.Domain;
using RenewalReminder.Service;

namespace RenewalReminder.Services.Abstract
{
    public interface ICentralService : IServiceBase
    {
        Task<Result<Central>> SaveCentral(Central entity);
        Task<Result<Guest>> SaveGuest(Guest entity);
        Task<Result<MarketPermit>> SaveMarketPermit(MarketPermit entity);

    }
}
