using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Models
{
    public class UserEdit
    {
   
        [Required,EmailAddress]
        public string Email { get; set; }
        [MinLength(4, ErrorMessage ="Minimum length is 4")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public UserEdit() { }
        public UserEdit(AppUser appUser)
        {
            //UserName = appUser.UserName;
            Email = appUser.Email;
            Password = appUser.PasswordHash;
        }
    }

    
}
