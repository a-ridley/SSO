using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace ServiceLayer.Services
{
    public class ApiKeyService: IApiKeyService
    {
        IApiKeyRepository _apiKeyRepository;

        public ApiKeyService(DatabaseContext _db)
        {
            _apiKeyRepository = new ApiKeyRepository(_db);
        }

        public ApiKey CreateKey(ApiKey key)
        {
            return _apiKeyRepository.CreateNewKey(key);
        }

        public ApiKey DeleteKey(Guid id)
        {
            return _apiKeyRepository.DeleteKey(id);
        }

        public ApiKey GetKey(Guid id)
        {
            return _apiKeyRepository.GetKey(id);
        }

        public ApiKey GetKey(string key)
        {
            return _apiKeyRepository.GetKey(key);
        }

        public ApiKey UpdateKey(ApiKey key)
        {
            return _apiKeyRepository.UpdateKey(key);
        }
        
    }
}
