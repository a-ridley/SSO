using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ServiceLayer.Services
{
    // This class provides a set of signing methods that conform to LTI spec
    public class SignatureService
    {
        // Signs a dictionary with the provided key by constructing a key/value string
        public string Sign(string key, Dictionary<string, string> payload)
        //public string Sign(string key, dynamic payload)
        {
            // Order the provided dictionary by key
            // This is necessary so that the recipient of the payload will be able to generate the
            // correct hash even if the order changes
            var orderedPayload = from payloadItem in payload
                orderby payloadItem.Value ascending
                select payloadItem;
            
            var payloadString = "";
            // Build a payload string with the format:
            // key =value;key2=value2;
            // SECURITY: This must be passed in this format so that the resulting hash is the same
            foreach (KeyValuePair<string, string> pair in orderedPayload)
            {
                payloadString = payloadString + pair.Key + "=" + pair.Value + ";";
            }
            
            var signature = Sign(key, payloadString);
            return signature;
        }

        // Signs a string with the provided key
        public string Sign(string key, string payloadString)
        {
            // Instantiate a new hashing algorithm with the provided key
            HMACSHA256 hashingAlg = new HMACSHA256(Encoding.ASCII.GetBytes(key));

            // Get the raw bytes from our payload string
            byte[] payloadBuffer = Encoding.ASCII.GetBytes(payloadString);

            // Calculate our hash from the byte array
            byte[] signatureBytes = hashingAlg.ComputeHash(payloadBuffer);

            var signature = Convert.ToBase64String(signatureBytes);
            return signature;
        }
    }
}
