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

        public IEnumerable GetAllApplications()
        {
            try
            {
                return _db.Applications.Select(app => new
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
