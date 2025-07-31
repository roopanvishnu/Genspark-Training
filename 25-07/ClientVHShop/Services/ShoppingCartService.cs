using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Services;

public class ShoppingCartService
{
    private OrderRepo _orderRepo;
    private OrderDetailRepo _orderDetailRepo;
    private ProductRepo _productRepo;
    public ShoppingCartService(OrderRepo orderRepo, OrderDetailRepo orderDetailRepo, ProductRepo productRepo)
    {
        _orderRepo = orderRepo;
        _orderDetailRepo = orderDetailRepo;
        _productRepo = productRepo;
    }

    public async Task<Order> Checkout([FromBody] CheckOutRequestDTO checkOutDTO)
    {
        if (checkOutDTO.CartItems == null || checkOutDTO.CartItems.Count() == 0)
        {
            throw new Exception("No Items to checkout");
        }
        Order order = new Order
        {
            CustomerName = checkOutDTO.CustomerName,
            CustomerEmail = checkOutDTO.CustomerEmail,
            CustomerAddress = checkOutDTO.CustomerAddress,
            CustomerPhone = checkOutDTO.CustomerPhone,
            OrderDate = DateTime.UtcNow,
            Status = "Processing",
            PaymentType = "Cash"
        };
        order = await _orderRepo.Add(order);

        foreach (CartItemDTO cart in checkOutDTO.CartItems)
        {
            var p = await _productRepo.Get(cart.ProductId);
            OrderDetail od = new OrderDetail
            {
                OrderID = order.OrderID,
                ProductID = cart.ProductId,
                Price = p.Price,
                Quantity = cart.Quantity
            };
            od = await _orderDetailRepo.Add(od);
        }
        return order;
    }
}