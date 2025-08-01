using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Services;

public class OrderDetailService
{
    private OrderDetailRepo _orderDetailRepo;
    public OrderDetailService(OrderDetailRepo orderDetailRepo)
    {
        _orderDetailRepo = orderDetailRepo;
    }
    
    public async Task<List<OrderDetail>> GetAll()
    {
        return (await _orderDetailRepo.GetAll()).ToList();
    }

    public async Task<List<OrderDetail>> GetByOrderId(int id)
    {
        return (await _orderDetailRepo.GetAll()).Where(od => od.OrderID == id).ToList();
    } 
  
}