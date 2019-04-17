using DataAccessLayer.Models;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public interface IApiKeyRepository
    {
        ApiKey CreateNewKey(ApiKey key);
        ApiKey DeleteKey(Guid id);
        ApiKey GetKey(Guid id);
        ApiKey GetKey(string key);
        List<ApiKey> GetAllKeys(Guid appId);
        ApiKey UpdateKey(ApiKey key);
    }
}
