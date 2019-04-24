using System;

namespace ManagerLayer.LaunchManagement
{
    // Represents the payload that will be POSTed to the destination server on launch
    public class LaunchPayload
    {
        // Included in signature
        public Guid ssoUserId { get; set; }
        public string email { get; set; }
        public long timestamp { get; set; }

        // Excluded from signature
        public string signature { get; set; }

        // Generate string to be signed
        public string PreSignatureString()
        {
            string acc = "";
            acc += "ssoUserId=" + ssoUserId + ";";
            acc += "email=" + email + ";";
            acc += "timestamp=" + timestamp + ";";
            return acc;
        }
    }
}
