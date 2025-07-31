using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Services;

public class OrderService
{
    private OrderRepo _orderRepo;
    public OrderService(OrderRepo orderRepo)
    {
        _orderRepo = orderRepo;
    }
    
    public async Task<List<Order>> GetAll()
    {
        return (await _orderRepo.GetAll()).ToList();
    }
    public async Task<List<Order>> GetPage(int? page)
    {
        var pageNumber = page ?? 1;
        var pageSize = 2;
        var orders = (await _orderRepo.GetAll())
                            .OrderByDescending(x => x.OrderID)
                            .Skip(pageSize * (pageNumber - 1))
                            .Take(pageSize)
                            .ToList();
        return orders;
    }

    public async Task<Order> Get(int id)
    {
        return (await _orderRepo.Get(id));
    }
    public async Task<Order> Create([FromBody] OrderAddDTO orderDTO)
    {
        var order = new Order
        {
            CustomerAddress = orderDTO.CustomerAddress,
            CustomerEmail = orderDTO.CustomerEmail,
            CustomerName = orderDTO.CustomerName,
            CustomerPhone = orderDTO.CustomerPhone,
            PaymentType = orderDTO.PaymentType,
            Status = orderDTO.Status??"Active"
        };
        order = await _orderRepo.Add(order);
        return order;
    }
    public async Task<Order> Edit(int id, [FromBody] OrderAddDTO orderDTO)
    {
        Order Order = await _orderRepo.Get(id);
        
        Order.CustomerAddress = orderDTO.CustomerAddress;
        Order.CustomerEmail = orderDTO.CustomerEmail;
        Order.CustomerName = orderDTO.CustomerName;
        Order.CustomerPhone = orderDTO.CustomerPhone;
        Order.PaymentType = orderDTO.PaymentType;
        Order.Status = orderDTO.Status;


        Order = await _orderRepo.Update(id, Order);
        return Order;
    }
    public async Task<Order> Delete(int id)
    {
        Order Order = await _orderRepo.Delete(id);
        return Order;
    }
}