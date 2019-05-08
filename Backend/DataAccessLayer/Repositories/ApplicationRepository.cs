using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System.Linq;
using System.Data.Entity;
using System.Collections;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public class ApplicationRepository: IApplicationRepository
    {
        DatabaseContext _db;

        public ApplicationRepository(DatabaseContext _db)
        {
            this._db = _db;
        }

        public Application CreateNewApplication(Application app)
        {
            var result = GetApplication(app.Title, app.Email);
            if (result != null)
            {
                return null;
            }
            _db.Entry(app).State = EntityState.Added;
            return app;

        }

        public Application DeleteApplication(Guid id)
        {
            var app = GetApplication(id);
            if (app == null)
            {
                return null;
            }

            _db.Entry(app).State = EntityState.Deleted;
            return app;

        }

        public Application GetApplication(Guid id)
        {
            var response = _db.Applications.Include(x => x.ApiKeys)
                .Where(a => a.Id == id)
                .FirstOrDefault<Application>();
            return response;
        }

        public Application GetApplication(string title, string email)
        {
            var app = _db.Applications.Include(x => x.ApiKeys)
                .Where(a => a.Title == title && a.Email == email)
                .FirstOrDefault<Application>();

            return app;
        }

        /// <summary>
        /// Gets a certain amount of applications based on the current page and page size.
        /// </summary>
        /// <param name="currentPage">The current page the user is on</param>
        /// <param name="pageSize">The number of applications to display</param>
        /// <param name="totalPages">The total number of pages to display</param>
        /// <returns>A collection of paginated applications along with the total number of pages</returns>
        public IEnumerable GetPaginatedApplications(int currentPage, int pageSize, out int totalPages)
        {
            // Calculates the row index of where to start the query from.
            // This will enable us to start at a certain row based on what page the user is on.
            // We subtract 1 to the current page because the query starts at index 0
            var startingIndex = pageSize * (currentPage - 1);

            // Perform a query to get all applications from the database
            var applications = _db.Applications
                // Order the query by the application id
                .OrderBy(app => app.Id)
                .Select(app => new
                {
                    app.Id,
                    app.Title,
                    app.Description,
                    app.Email,
                    app.LogoUrl,
                    app.LaunchUrl,
                    app.UnderMaintenance,
                    app.ClickCount
                }).ToList();

            // Get the total number of rows from the Application table
            int totalApplicationCount = applications.Count();
            // Calculates total number of pages for the pagination
            totalPages = (int) Math.Ceiling((double) totalApplicationCount / pageSize);

            // Starting from the starting index row, take the amount of 
            // applications that the user wants to display
            return applications.Skip(startingIndex).Take(pageSize);
        }

        public IEnumerable SortAllApplicationsAlphaAscending()
        {
            try
            {
                return _db.Applications.OrderBy(app => app.Title).Select(app => new
                {
                    Id = app.Id,
                    LaunchUrl = app.LaunchUrl,
                    Title = app.Title,
                    Email = app.Email,
                    LogoUrl = app.LogoUrl,
                    Description = app.Description,
                    UnderMaintenance = app.UnderMaintenance,
                    ClickCount = app.ClickCount
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable SortAllApplicationsAlphaDescending()
        {
            try
            {
                return _db.Applications.OrderByDescending(app => app.Title).Select(app => new
                {
                    Id = app.Id,
                    LaunchUrl = app.LaunchUrl,
                    Title = app.Title,
                    Email = app.Email,
                    LogoUrl = app.LogoUrl,
                    Description = app.Description,
                    UnderMaintenance = app.UnderMaintenance,
                    ClickCount = app.ClickCount
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable SortAllApplicationsNumOfClicks()
        {
            try
            {
                return _db.Applications.OrderByDescending(app => app.ClickCount).Select(app => new
                {
                    Id = app.Id,
                    LaunchUrl = app.LaunchUrl,
                    Title = app.Title,
                    Email = app.Email,
                    LogoUrl = app.LogoUrl,
                    Description = app.Description,
                    UnderMaintenance = app.UnderMaintenance,
                    ClickCount = app.ClickCount
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Application> GetAllApplicationsList()
        {
            try
            {
                return _db.Applications.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Application UpdateApplication(Application app)
        {
            var result = GetApplication(app.Id);
            if (result == null)
            {
                return null;
            }
            _db.Entry(app).State = EntityState.Modified;
            return result;
        }

    }
}
