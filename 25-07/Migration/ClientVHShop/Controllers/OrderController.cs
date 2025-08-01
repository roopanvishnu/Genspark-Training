using System.Text;
using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private OrderService _orderService;
    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetAll()
    {
        try
        {
            var order = (await _orderService.GetAll()).ToList();
            return Ok(order);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpGet("page/{page}")]
    public async Task<ActionResult<List<Order>>> GetPage(int page)
    {
        try
        {
            var order = (await _orderService.GetPage(page)).ToList();
            return Ok(order);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("Details/{id}")]
    public async Task<ActionResult<Order>> Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }
        try
        {
            var order = await _orderService.Get((int)id);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
        
    }   
    [HttpPost("Create")]
    public async Task<ActionResult<Order>> Create([FromBody] OrderAddDTO orderDTO)
    {
        try
        {
            var order = await _orderService.Create(orderDTO);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }   
    [HttpPost("Edit/{id}")]
    public async Task<ActionResult<Order>> Edit(int id,[FromBody] OrderAddDTO orderDTO)
    {
        try
        {
            var order = await _orderService.Edit(id,orderDTO);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }   
    [HttpPost("Delete/{id}")]
    public async Task<ActionResult<Order>> Delete(int id)
    {
        try
        {
            var order = await _orderService.Delete(id);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }   
}