using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public interface IApplicationRepository
    {
        Application CreateNewApplication(Application app);
        Application DeleteApplication(Guid id);
        Application GetApplication(Guid id);
        Application GetApplication(string title, string email);
        IEnumerable GetPaginatedApplications(int currentPage, int pageSize, out int totalPages);
        IEnumerable SortAllApplicationsAlphaAscending();
        IEnumerable SortAllApplicationsAlphaDescending();
        IEnumerable SortAllApplicationsNumOfClicks();
        List<Application> GetAllApplicationsList();
        Application UpdateApplication(Application app);
    }
}
