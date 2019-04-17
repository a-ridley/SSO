using System;
using System.Collections;
using System.Collections.Generic;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;


namespace ServiceLayer.Services
{
    public class ApplicationService: IApplicationService
    {
        IApplicationRepository _applicationRepository;
        IApiKeyRepository _apiKeyRepository;

        DatabaseContext _db;

        public ApplicationService(DatabaseContext _db)
        {
            this._db = _db;
            _applicationRepository = new ApplicationRepository(_db);
            _apiKeyRepository = new ApiKeyRepository(_db);
        }

        public Application CreateApplication(Application app)
        {
            return _applicationRepository.CreateNewApplication(app);
        }

        public Application DeleteApplication(Guid id)
        {
            // Delete children api keys first
            List<ApiKey> keys = _apiKeyRepository.GetAllKeys(id);
            foreach(ApiKey key in keys)
            {
                _apiKeyRepository.DeleteKey(key.Id);
            }

            return _applicationRepository.DeleteApplication(id);
        }

        public Application GetApplication(Guid id)
        {
            return _applicationRepository.GetApplication(id);
        }

        public Application GetApplication(string title, string email)
        {
            return _applicationRepository.GetApplication(title, email);
        }

        public List<Application> GetAllApplicationsList()
        {
            return _applicationRepository.GetAllApplicationsList();
        }
       
        public IEnumerable GetAllApplications()
        {
            return _applicationRepository.GetAllApplications();
        }
        
        public IEnumerable SortAllApplicationsAlphaAscending()
        {
            return _applicationRepository.SortAllApplicationsAlphaAscending();
        }

        public IEnumerable SortAllApplicationsNumOfClicks()
        {
            return _applicationRepository.SortAllApplicationsNumOfClicks();
        }

        public IEnumerable SortAllApplicationsAlphaDescending()
        {
            return _applicationRepository.SortAllApplicationsAlphaDescending();
        }

        public Application UpdateApplication(Application app)
        {
            return _applicationRepository.UpdateApplication(app);
        }

    }
}
