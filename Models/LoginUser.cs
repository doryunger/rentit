using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarRent.Entities;
using System.Linq;
using System.Web;

namespace RentIt.Models
{
    public class LoginUser
    {
        [Required]
        [StringLength(12, MinimumLength = 3, ErrorMessage = "Username must have minimum 3-12 characters")]
        public string Username { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "Password must have 6-12 characters")]
        public string Password { get; set; }


        public User converttoUser(LoginUser lvm)
        {
            return new User
            {
                UserName = Username,

                Password = Password

            };
        }
    }
}