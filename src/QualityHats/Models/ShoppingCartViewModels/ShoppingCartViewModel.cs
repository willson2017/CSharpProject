using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityHats.Models.ShoppingCartViewModels
{
    public class ShoppingCartViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal CartTotal { get; set; }
        public decimal CartGST { get; set; }
        public decimal CartSub { get; set; }


    }
}
