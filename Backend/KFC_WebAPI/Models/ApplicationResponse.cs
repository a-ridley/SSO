using System;
using System.Collections;
using System.Linq;
using System.Web;

namespace KFC_WebAPI.Models
{
    public class ApplicationResponse
    {
        public int TotalPages { get; private set; }
        public IEnumerable PaginatedApplications { get; private set; }

        public ApplicationResponse(int totalPages, IEnumerable paginatedApplications)
        {
            TotalPages = totalPages;
            PaginatedApplications = paginatedApplications;
        }
    }
}