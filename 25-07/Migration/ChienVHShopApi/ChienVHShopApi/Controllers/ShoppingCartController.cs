using ChienVHShopAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ChienVHShopAPI.Contexts;
using PayPal.v1.Payments;
using Order = ChienVHShopAPI.Models.Order;

namespace ChienVHShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartApiController : ControllerBase
    {
        private readonly ChienVHShopDbContext _context;
        private Payment payment;

        public ShoppingCartApiController(ChienVHShopDbContext context)
        {
            _context = context;
        }

        // POST: api/ShoppingCart/Checkout
        [HttpPost("checkout")]
        public IActionResult Checkout([FromBody] CheckoutRequest request)
        {
            if (request?.CartItems == null || !request.CartItems.Any())
                return BadRequest("Cart is empty.");

            // 1. Save Order
            var order = new Order
            {
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerEmail = request.CustomerEmail,
                CustomerAddress = request.CustomerAddress,
                OrderDate = DateTime.Now,
                PaymentType = "Cash",
                Status = "Processing"
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // 2. Save Order Details
            foreach (var item in request.CartItems)
            {
                var product = _context.Products.Find(item.ProductId);
                if (product == null) continue;

                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                };

                _context.OrderDetails.Add(orderDetail);
            }

            _context.SaveChanges();

            return Ok(new { Message = "Order placed successfully", OrderId = order.OrderId });
        }


        // GET: api/ShoppingCart/paypal-callback
        
        
    }

    // Request models
    public class CheckoutRequest
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public List<CartItemRequest> CartItems { get; set; }
    }

    public class CartItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
