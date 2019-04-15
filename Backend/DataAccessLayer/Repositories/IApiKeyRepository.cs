using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;

namespace DataAccessLayer.Repositories
{
    public interface IApiKeyRepository
    {
        ApiKey CreateNewKey(DatabaseContext _db, ApiKey key);
        ApiKey DeleteKey(DatabaseContext _db, Guid id);
        ApiKey GetKey(DatabaseContext _db, Guid id);
        ApiKey GetKey(DatabaseContext _db, Guid applicationId, bool isUsed);
        ApiKey GetKey(DatabaseContext _db, string key);
        ApiKey UpdateKey(DatabaseContext _db, ApiKey key);
    }
}
