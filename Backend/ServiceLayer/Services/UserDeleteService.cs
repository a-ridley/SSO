using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using System.Security.Cryptography;

namespace ServiceLayer.Services
{
    public class UserDeleteService
    {
        public async Task<HttpResponseMessage> SendDeleteRequest(string deleteURL, Dictionary<string, string> payload)
        {
            HttpClient client = new HttpClient();
            
            var stringPayload = JsonConvert.SerializeObject(payload);
            var jsonPayload = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var request = await client.PostAsync(deleteURL, jsonPayload);
            return request;
        }

    }
}