using System;
using System.ComponentModel.DataAnnotations;

namespace KFC_WebAPI.RequestModels
{
    public class ExternalUserDeleteRequestModel
    {
        // Included in signature
        [Required]
        public string appId { get; set; }
        [Required]
        public Guid ssoUserId { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public long timestamp { get; set; }

        [Required]
        public string signature { get; set; }

    }
}