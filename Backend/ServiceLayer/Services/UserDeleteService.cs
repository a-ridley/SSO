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
    public class UserDeletePayload
    {
        // Included in signature
        public string SsoId { get; set; }
        public string Email { get; set; }
        public long Timestamp { get; set; }

        // Excluded from signature
        public string Signature { get; set; }

        // Generate string to be signed
        public string PreSignatureString()
        {
            string acc = "";
            acc += "ssoUserId=" + SsoId + ";";
            acc += "email=" + Email + ";";
            acc += "timestamp=" + Timestamp + ";";
            return acc;
        }
    }


    public class UserDeleteService
    {
        public async Task<HttpResponseMessage> SendDeleteRequest(string deleteurl, string appkey, string ssoId)
        {
            HttpClient client = new HttpClient();
            UserDeletePayload deletePayload = new UserDeletePayload
            {
                Email = "email@email.com",
                SsoId = ssoId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };
            HMACSHA256 hmacsha1 = new HMACSHA256(Encoding.ASCII.GetBytes(appkey));
            byte[] deletePayloadBuffer = Encoding.ASCII.GetBytes(deletePayload.PreSignatureString());
            byte[] signatureBytes = hmacsha1.ComputeHash(deletePayloadBuffer);
            deletePayload.Signature = Convert.ToBase64String(signatureBytes);

            var stringPayload = JsonConvert.SerializeObject(deletePayload);
            var jsonPayload = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var request = await client.PostAsync(deleteurl, jsonPayload);
            return request;
        }

        public async Task<HttpResponseMessage> SendDeleteTest(string ssoId)
        {
            HttpClient client = new HttpClient();
            UserDeletePayload payload = new UserDeletePayload
            {
                Email = "email@email.com",
                SsoId = ssoId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            var stringPayload = JsonConvert.SerializeObject(payload);
            var jsonPayload = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var request = await client.PostAsync("http://localhost:52324/api/sso/deleteuser", jsonPayload);
            return request;
        }
    }
}