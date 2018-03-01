using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QualityHats.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "SupplierName")]
        public string SupplierName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 7)]
        [Phone]
        [Display(Name = "Mobile Phone")]
        public string MobilePhone { get; set; }

        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 7)]
        [Phone]
        [Display(Name = "Work Phone")]
        public string WorkPhone { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
