using System;
using System.ComponentModel.DataAnnotations;

namespace BlogLab.Models.Account
{
    public class ApplicationUserLogin
    {
        [Required(ErrorMessage = "Username is required!")]
        [MinLength(5, ErrorMessage = "Username must be 5-20 characters!")]
        [MaxLength(20, ErrorMessage = "Username must be 5-20 characters!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [MinLength(10, ErrorMessage = "Password must be 10-50 characters!")]
        [MaxLength(50, ErrorMessage = "Password must be 10-50 characters!")]
        public string Password { get; set; }
    }
}
