using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private ShoppingCartService _shoppingCartService;
    public ShoppingCartController(ShoppingCartService shoppingCartService)
    {
        _shoppingCartService = shoppingCartService;
    }
    [HttpPost("Checkout")]
    public async Task<ActionResult<Order>> Checkout([FromBody] CheckOutRequestDTO dto)
    {
        try
        {
            var order = await _shoppingCartService.Checkout(dto);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}