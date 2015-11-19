using System;
using System.ComponentModel.DataAnnotations;

namespace VerIDial.Workshop.Models
{
    /// <summary>
    /// Submit number to initiate a request
    /// </summary>
    public class DemoModel
    {
        [Display(Name = "Phone Number"), Phone]
        public string PhoneNumber { get; set; }        
        public string submit { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Token Response/Request
    /// </summary>
    public class TokenDTO
    {
        public Guid Token { get; set; }
    }

    /// <summary>
    /// Status Response
    /// </summary>
    public class StatusDTO
    {
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
    }

    /// <summary>
    /// Model to return a simple message
    /// </summary>
    public class MessageModel
    {
        public string Message { get; set; }
    }
}
