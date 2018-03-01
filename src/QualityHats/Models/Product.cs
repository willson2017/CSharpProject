using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace QualityHats.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        public int SupplierID { get; set; }
        public int CategoryID { get; set; }   
           
        public string Description { get; set; }
        public string ImagePath { get; set; }

        public Supplier Supplier { get; set; }
        public Category Category { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
