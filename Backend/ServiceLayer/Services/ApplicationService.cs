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
        // Repository
        IApplicationRepository _applicationRepository;

        public ApplicationService()
        {
            _applicationRepository = new ApplicationRepository();
        }

        /// <summary>
        /// Call the application repository to create a new application record
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="app">application</param>
        /// <returns>The application created</returns>
        public Application CreateApplication(DatabaseContext _db, Application app)
        {
            return _applicationRepository.CreateNewApplication(_db, app);
        }

        /// <summary>
        /// Call the application repository to delete an application record
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="url">application url</param>
        /// <returns>The deleted application</returns>
        public Application DeleteApplication(DatabaseContext _db, Guid id)
        {
            return _applicationRepository.DeleteApplication(_db, id);
        }

        /// <summary>
        /// Call the application repository to retrieve an application record by id
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="url">application</param>
        /// <returns>The retrieved application</returns>
        public Application GetApplication(DatabaseContext _db, Guid id)
        {
            return _applicationRepository.GetApplication(_db, id);
        }

        /// <summary>
        /// Call the application repository to retrieve an application record by title and email
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="title">application title</param>
        /// <param name="email">email</param>
        /// <returns></returns>
        public Application GetApplication(DatabaseContext _db, string title, string email)
        {
            return _applicationRepository.GetApplication(_db, title, email);
        }

        /// <summary>
        /// Call the application repository to return all applications registered with the SSO
        /// </summary>
        /// <param name="_db">database</param>
        /// <returns>All applications registered with the SSO</returns>
        public List<Application> GetAllApplicationsList(DatabaseContext _db)
        {
            return _applicationRepository.GetAllApplicationsList(_db);
        }

        /// <summary>
        /// Call the application repository to return all applications registered with the SSO
        /// </summary>
        /// <param name="_db">database</param>
        /// <returns>All applications registered with the SSO</returns>
        public IEnumerable GetAllApplications(DatabaseContext _db)
        {
            return _applicationRepository.GetAllApplications(_db);
        }

        /// <summary>
        /// Call the application repository to return all sorted applications registered with the SSO
        /// </summary>
        /// <param name="_db">database</param>
        /// <returns>All sorted applications registered with the SSO</returns>
        public IEnumerable SortAllApplicationsAlphaAscending(DatabaseContext _db)
        {
            return _applicationRepository.SortAllApplicationsAlphaAscending(_db);
        }

        /// <summary>
        /// Call the application repository to return all sorted applications by number of clicks
        /// </summary>
        /// <param name="_db">database</param>
        /// <returns>All sorted applications registered with the SSO</returns>
        public IEnumerable SortAllApplicationsNumOfClicks(DatabaseContext _db)
        {
            return _applicationRepository.SortAllApplicationsNumOfClicks(_db);
        }

        /// <summary>
        /// Call the application repository to return all sorted applications registered with the SSO
        /// </summary>
        /// <param name="_db">database</param>
        /// <returns>All sorted applications registered with the SSO</returns>
        public IEnumerable SortAllApplicationsAlphaDescending(DatabaseContext _db)
        {
            return _applicationRepository.SortAllApplicationsAlphaDescending(_db);
        }

        /// <summary>
        /// Call the application repository to update an application record
        /// </summary>
        /// <param name="_db">database</param>
        /// <param name="app">application</param>
        /// <returns>The updated application</returns>
        public Application UpdateApplication(DatabaseContext _db, Application app)
        {
            return _applicationRepository.UpdateApplication(_db, app);
        }

    }
}
