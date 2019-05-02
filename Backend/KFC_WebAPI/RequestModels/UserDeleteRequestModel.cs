using System;
using System.ComponentModel.DataAnnotations;

namespace KFC_WebAPI.RequestModels
{
    public class UserDeleteRequestModel
    {
        [Required]
        public string token { get; set; }
    }
}