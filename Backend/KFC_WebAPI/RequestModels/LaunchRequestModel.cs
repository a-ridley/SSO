using System;
using System.ComponentModel.DataAnnotations;

namespace KFC_WebAPI.RequestModels
{
    public class LaunchRequest
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public Guid AppId { get; set; }
    }
}
