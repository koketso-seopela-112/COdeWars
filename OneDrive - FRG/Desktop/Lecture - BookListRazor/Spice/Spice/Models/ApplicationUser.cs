using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class ApplicationUser: IdentityUser
    {

        [Required]
        [StringLength(100, ErrorMessage = "Full name(s) must not be empty and should be as seen on your ID book/Card", ErrorMessageResourceName = null, ErrorMessageResourceType = null, MinimumLength = 3)]
        public string FullName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Last Name must not be empty and should be as seen on your ID book/Card", ErrorMessageResourceName = null, ErrorMessageResourceType = null, MinimumLength = 3)]
        public string LastName { get; set; }

        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }
}
