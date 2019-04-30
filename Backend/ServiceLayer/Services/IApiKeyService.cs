using DataAccessLayer.Models;
using System;

namespace ServiceLayer.Services
{
    public interface IApiKeyService
    {
        ApiKey CreateKey(ApiKey key);
        ApiKey DeleteKey(Guid id);
        ApiKey GetKey(Guid id);
        ApiKey GetKey(string key);
        ApiKey UpdateKey(ApiKey key);
    }
}
