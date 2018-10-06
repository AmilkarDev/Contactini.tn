using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IdentitySample.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        // Add the Address Info:
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        // Use a sensible display name for views:
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public string PhotoLink { get; set; }
        public bool isAdmin { get;set;}
        public bool isServiceProvider { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}