using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Models
{
    public class Login
    {
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, MinLength(4, ErrorMessage = "Minimum length is 4")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string  ReturnUrl { get; set; }


    }
}
