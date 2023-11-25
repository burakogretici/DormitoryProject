using RenewalReminder.Domain;
using System;

namespace RenewalReminder.Services.Abstract
{
    public interface IUserAccessor
    {
        bool IsLogined { get; }
        User User { get; set; }
        string ClientIP { get; }
        string RequestLink { get; }

        void Store<T>(string key, T data);
        T Get<T>(string key);
        void Clear(string key = null);
    }
}
