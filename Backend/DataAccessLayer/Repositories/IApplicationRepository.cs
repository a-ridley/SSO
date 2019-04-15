using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public interface IApplicationRepository
    {
        Application CreateNewApplication(DatabaseContext _db, Application app);
        Application DeleteApplication(DatabaseContext _db, Guid id);
        Application GetApplication(DatabaseContext _db, Guid id);
        Application GetApplication(DatabaseContext _db, string title, string email);
        IEnumerable GetAllApplications(DatabaseContext _db);
        IEnumerable SortAllApplicationsAlphaAscending(DatabaseContext _db);
        IEnumerable SortAllApplicationsAlphaDescending(DatabaseContext _db);
        IEnumerable SortAllApplicationsNumOfClicks(DatabaseContext _db);
        List<Application> GetAllApplicationsList(DatabaseContext _db);
        Application UpdateApplication(DatabaseContext _db, Application app);
    }
}
