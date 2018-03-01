using Microsoft.AspNetCore.Mvc;
using QualityHats.Data;
using QualityHats.Models;
using QualityHats.Models.ShoppingCartViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityHats.ViewComponents
{
    public class ShoppingCartViewModelViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartViewModelViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            return View(ReturnCurrentCartViewModel());
        }

        public ShoppingCartViewModel ReturnCurrentCartViewModel()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(_context),
                CartGST = cart.GetGst(_context),
                CartSub = cart.GetTotalSubAmount(_context),
                CartTotal = cart.GetTotalAmount(_context)
            };
            return viewModel;
        }

    }
}
