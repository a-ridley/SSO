using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public interface IApiKeyService
    {
        ApiKey CreateKey(DatabaseContext _db, ApiKey key);
        ApiKey DeleteKey(DatabaseContext _db, Guid id);
        ApiKey GetKey(DatabaseContext _db, Guid id);
        ApiKey GetKey(DatabaseContext _db, Guid applicationId, bool isUsed);
        ApiKey GetKey(DatabaseContext _db, string key);
        ApiKey UpdateKey(DatabaseContext _db, ApiKey key);
    }
}
