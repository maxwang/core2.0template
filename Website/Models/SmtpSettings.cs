using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class SmtpSettings
    {
        public string EmailHost { get; set; }
        public string EmailPort { get; set; }
        public string EmailServerUsername { get; set; }
        public string EmailServerPassword { get; set; }
        public string FeedbackEmailMailTo { get; set; }
        public string FeedbackEmailMailFrom { get; set; }
        public string NormalMailTo { get; set; }
        public string NormalMailFrom { get; set; }
        public string Feedback { get; set; }
    }

    public class BitDefenderSignUpModel
    {
        public string AccountName { get; set; }
        public string ContactName { get; set; }
        public string TerritoryName { get; set; }
        public string BDTC { get; set; }
        public string MSPTC { get; set; }
    }

    public class ContactViewModel
    {
        
        [StringLength(20, MinimumLength = 5)]
        public string Name { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}



