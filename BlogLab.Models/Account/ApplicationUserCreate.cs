using System;
using System.ComponentModel.DataAnnotations;

namespace BlogLab.Models.Account
{
    public class ApplicationUserCreate : ApplicationUserLogin
    {
        [MinLength(10, ErrorMessage = "Password must be 10-30 characters!")]
        [MaxLength(30, ErrorMessage = "Password must be 10-30 characters!")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [MaxLength(50, ErrorMessage = "Password can be at most 50 characters!")]
        [EmailAddress(ErrorMessage = "Invalid email format!")]
        public string Email { get; set; }

    }
}

