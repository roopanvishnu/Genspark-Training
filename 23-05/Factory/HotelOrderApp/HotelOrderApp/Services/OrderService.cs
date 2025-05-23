using HotelOrderApp.Interfaces;
using HotelOrderApp.Models;
using System;
using System.Collections.Generic;

namespace HotelOrderApp.Infrastructure
{
    public class OrderService : IOrderService
    {
        private readonly IFoodFactory _factory;
        private readonly List<Order> _orders = new();

        public OrderService(IFoodFactory factory)
        {
            _factory = factory;
        }

        public void PlaceOfOrder(string foodtype)
        {
            try
            {
                var food = _factory.CreateFood(foodtype);
                _orders.Add(new Order { FoodItem = food, OrderAt = DateTime.Now });
                Console.WriteLine($"{food.Name} ordered successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
       
        

        public void ShowOrders()
        {
            Console.WriteLine("\n--- Order List ---");
            foreach (var order in _orders)
            {
                Console.WriteLine($"{order.FoodItem.Name} - ₹{order.FoodItem.Price} at {order.OrderAt}");
            }
        }
    }
}
