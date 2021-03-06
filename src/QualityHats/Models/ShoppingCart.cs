﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QualityHats.Data;
using Microsoft.AspNetCore.Http;


namespace QualityHats.Models
{
    public class ShoppingCart
    {
        public string ShoppingCartId { get; set; }
        public const string CartSessionKey = "cartId";

        public static ShoppingCart GetCart(HttpContext context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        public void AddToCart(Product product, ApplicationDbContext db)
        {
            var cartItem = db.CartItems.SingleOrDefault(c => c.CartID == ShoppingCartId && c.Product.ProductID == product.ProductID);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    Product = product,
                    CartID = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                db.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Count++;
            }
            db.SaveChanges();
        }

        public int RemoveFromCart(int id, ApplicationDbContext db)
        {
            var cartItem = db.CartItems.SingleOrDefault(cart => cart.CartID == ShoppingCartId && cart.Product.ProductID == id);
            int itemCount = 0;
            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    db.CartItems.Remove(cartItem);
                }
                db.SaveChanges();
            }
            return itemCount;
        }

        public void EmptyCart(ApplicationDbContext db)
        {
            var cartItems = db.CartItems.Where(cart => cart.CartID == ShoppingCartId);
            foreach (var cartItem in cartItems)
            {
                db.CartItems.Remove(cartItem);
            }
            db.SaveChanges();
        }

        public List<CartItem> GetCartItems(ApplicationDbContext db)
        {
            List<CartItem> cartItems = db.CartItems.Include(i => i.Product).Where(cartItem => cartItem.CartID == ShoppingCartId).ToList();

            return cartItems;

        }

        public string GetCartId(HttpContext context)
        {
            if (context.Session.GetString(CartSessionKey) == null)
            {
                Guid tempCartId = Guid.NewGuid();
                context.Session.SetString(CartSessionKey, tempCartId.ToString());
            }
            return context.Session.GetString(CartSessionKey).ToString();
        }

        public int GetCount(ApplicationDbContext db)
        {
            int? count =
                (from cartItems in db.CartItems where cartItems.CartID == ShoppingCartId select (int?)cartItems.Count).Sum();
            return count ?? 0;
        }

        public decimal GetTotalSubAmount(ApplicationDbContext db) //Sub Amount
        {
            decimal? totalsubamount = (from cartItems in db.CartItems
                              where cartItems.CartID == ShoppingCartId
                              select (int?)cartItems.Count * cartItems.Product.UnitPrice).Sum();
            return totalsubamount ?? decimal.Zero;
        }

        public decimal GetGst(ApplicationDbContext db)
        {
            var faxrate = 0.15;
            decimal TotalSubAmount = GetTotalSubAmount(db);
            decimal? gst = Math.Round(decimal.Multiply(TotalSubAmount, (decimal)faxrate), 2);;
            return gst ?? decimal.Zero;
        }

        public decimal GetTotalAmount(ApplicationDbContext db)
        {
            decimal? total = Math.Round(GetTotalSubAmount(db) + GetGst(db), 2);
            return total ?? decimal.Zero;
        }

    }
}
