using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System.Linq;
using System.Data.Entity;

namespace DataAccessLayer.Repositories
{
    public class ApiKeyRepository: IApiKeyRepository
    {
        /// <summary>
        /// Create a new Api Key record
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="key">api key</param>
        /// <returns>created api key</returns>
        public ApiKey CreateNewKey(DatabaseContext _db, ApiKey key)
        {
            var apiKey = GetKey(_db, key.Key);
            if (apiKey != null)
            {
                return null;
            }
            _db.Entry(key).State = EntityState.Added;
            return key;

        }

        /// <summary>
        /// Delete an Api Key record
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="id">api key id</param>
        /// <returns>deleted api key</returns>
        public ApiKey DeleteKey(DatabaseContext _db, Guid id)
        {
            var apiKey = GetKey(_db, id);
            if (apiKey == null)
            {
                return null;
            }
            _db.Entry(apiKey).State = EntityState.Deleted;
            return apiKey;
        }

        /// <summary>
        /// Retrieve an Api Key record by id field
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="id">api key id</param>
        /// <returns>The retrieved api key</returns>
        public ApiKey GetKey(DatabaseContext _db, Guid id)
        {
            var response = _db.Keys.Find(id);
            return response;
        }

        /// <summary>
        /// Retrieve an Api Key record by application id and isUsed
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="appId">application id</param>
        /// <param name="isUsed">whether the key has been used or not</param>
        /// <returns></returns>
        public ApiKey GetKey(DatabaseContext _db, Guid applicationId, bool isUsed)
        {
            var apiKey = _db.Keys
                .Where(k => k.ApplicationId == applicationId && k.IsUsed == false)
                .FirstOrDefault<ApiKey>();
            return apiKey;
        }

        /// <summary>
        /// Retrieve an Api Key record by key field
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="key">key value of api key</param>
        /// <returns></returns>
        public ApiKey GetKey(DatabaseContext _db, string key)
        {
            var apiKey = _db.Keys
                .Where(k => k.Key == key)
                .FirstOrDefault<ApiKey>();
            return apiKey;
        }

        /// <summary>
        /// Update an Api Key record
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="key">api key</param>
        /// <returns></returns>
        public ApiKey UpdateKey(DatabaseContext _db, ApiKey key)
        {
            var result = GetKey(_db, key.Id);
            if (result == null)
            {
                return null;
            }
            _db.Entry(key).State = EntityState.Modified;
            return result;
        }

    }
}
