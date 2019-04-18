using DataAccessLayer.Models;
using DataAccessLayer.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.ApplicationManagement
{
    public interface IApplicationManager
    {
        HttpResponseContent ValidateRegistration(ApplicationRequest request);
        HttpResponseContent ValidatePublish(ApplicationRequest request);
        HttpResponseContent ValidateKeyGeneration(ApplicationRequest request);
        HttpResponseContent ValidateDeletion(ApplicationRequest request);
        HttpResponseContent ValidateUpdate(ApplicationRequest request);
        bool IsValidStringLength(string s, int length);
        bool IsValidEmail(string email);
        bool IsValidUrl(string url, ref Uri uriResult);
        bool IsValidImageExtension(Uri imageUrl, string type);
        bool IsValidDimensions(Uri imgUrl, int width, int height);
        void SaveChanges(object responses);
        void SendAppRegistrationEmail(string receiverEmail, string apiKey, string sharedSecretKey, Guid appId);
        void SendAppPublishEmail(string receiverEmail, Application app);
        void SendNewApiKeyEmail(string receiverEmail, string apiKey);
        void SendAppDeleteEmail(string receiverEmail, string appTitle);

    }
}
