using System.Text;
using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrderDetailController : ControllerBase
{
    private OrderDetailService _orderDetailService;
    public OrderDetailController(OrderDetailService orderDetailService)
    {
        _orderDetailService = orderDetailService;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDetail>>> GetAll()
    {
        try
        {
            var order = (await _orderDetailService.GetAll()).ToList();
            return Ok(order);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("Details/{id}")]
    public async Task<ActionResult<List<OrderDetail>>> Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }
        try
        {
            var order = await _orderDetailService.GetByOrderId((int)id);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }

    }
}