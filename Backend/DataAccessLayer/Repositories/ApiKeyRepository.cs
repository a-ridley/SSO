using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public class ApiKeyRepository: IApiKeyRepository
    {
        DatabaseContext _db;

        public ApiKeyRepository(DatabaseContext _db)
        {
            this._db = _db;
        }

        public ApiKey CreateNewKey(ApiKey key)
        {
            var apiKey = GetKey(key.Key);
            if (apiKey != null)
            {
                return null;
            }
            _db.Entry(key).State = EntityState.Added;
            return key;

        }

        public ApiKey DeleteKey(Guid id)
        {
            var apiKey = GetKey(id);
            if (apiKey == null)
            {
                return null;
            }
            _db.Entry(apiKey).State = EntityState.Deleted;
            return apiKey;
        }
        
        public ApiKey GetKey(Guid id)
        {
            var response = _db.Keys.Find(id);
            return response;
        }


        public ApiKey GetKey(string key)
        {
            var apiKey = _db.Keys
                .Where(k => k.Key == key)
                .FirstOrDefault<ApiKey>();
            return apiKey;
        }

        public List<ApiKey> GetAllKeys(Guid appId)
        {
            return _db.Keys
            .Where(x => x.ApplicationId == appId)
            .ToList();
        }

        public ApiKey UpdateKey(ApiKey key)
        {
            var result = GetKey(key.Id);
            if (result == null)
            {
                return null;
            }
            _db.Entry(key).State = EntityState.Modified;
            return result;
        }

    }
}
