using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ServiceLayer.Services
{
    public interface IApplicationService
    {
        Application CreateApplication(DatabaseContext _db, Application app);
        Application DeleteApplication(DatabaseContext _db, Guid id);
        Application GetApplication(DatabaseContext _db, Guid id);
        Application GetApplication(DatabaseContext _db, string title, string email);
        List<Application> GetAllApplicationsList(DatabaseContext _db);
        IEnumerable GetAllApplications(DatabaseContext _db);
        IEnumerable SortAllApplicationsAlphaAscending(DatabaseContext _db);
        IEnumerable SortAllApplicationsNumOfClicks(DatabaseContext _db);
        IEnumerable SortAllApplicationsAlphaDescending(DatabaseContext _db);
        Application UpdateApplication(DatabaseContext _db, Application app);
    }
}
