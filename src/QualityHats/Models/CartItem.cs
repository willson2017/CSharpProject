using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QualityHats.Models
{
    public class CartItem
    {
        [Key]
        public int ID { get; set; }
        //CartID
        public string CartID { get; set; }
        //Numbers
        public int Count { get; set; }
        //Date created
        public DateTime DateCreated { get; set; }

        //Navigation Property
        public Product Product { get; set; }
    }
}
