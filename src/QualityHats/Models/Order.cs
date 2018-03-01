using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QualityHats.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        public string ProductName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string Phone { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal GST { get; set; } //tax

        [Required]
        [DataType(DataType.Currency)]
        public decimal GrandTotal { get; set; } //accumulation

        [Required]
        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

        public DateTime OrderDate { get; set; }

        //Navigation Property
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ApplicationUser User { get; set; }
    }
}
