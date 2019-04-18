using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.ApplicationManagement
{
    public class HttpResponseContent
    {
        public HttpResponseContent()
        {

        }

        public HttpResponseContent(string message)
        {
            Message = message;
        }

        public HttpResponseContent(string message, string key)
        {
            Key = key;
            Message = message;
        }

        public HttpResponseContent(string message, string key, string secretKey, Guid appId)
        {
            Message = message;
            Key = key;
            SharedSecretKey = secretKey;
            AppId = appId;
        }

        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
        public string Key { get; set; }
        public string SharedSecretKey { get; set; }
        public Guid AppId { get; set; }
    }
}
