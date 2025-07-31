using System.Security.Claims;
using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private ProductService _productService;
    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<Product>>> GetAll()
    {
        try
        {
            var product = (await _productService.GetAll()).ToList();
            return Ok(product);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetByFilter(int? page, int? category)
    {
        try
        {
            var product = (await _productService.GetByFilter(page, category)).ToList();
            return Ok(product);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("Details/{id}")]
    public async Task<ActionResult<Product>> Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }
        try
        {
            var product = await _productService.Get((int)id);
            return Ok(product);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }

    }

    [Authorize]
    [HttpPost("Create")]
    public async Task<ActionResult<Product>> Create([FromBody] ProductAddDTO productDTO)
    {
        try
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            productDTO.UserId = int.Parse(userId!);
            var product = await _productService.Create(productDTO);
            return Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }   

    [Authorize]
    [HttpPost("Edit/{id}")]
    public async Task<ActionResult<Product>> Edit(int id,[FromBody] ProductAddDTO productDTO)
    {
        try
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (id != userId)
            {
                throw new Exception("UnAuthorized Access");
            }
            var product = await _productService.Edit(id,productDTO);
            return Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }   
    [Authorize]
    [HttpPost("Delete/{id}")]
    public async Task<ActionResult<Product>> Delete(int id)
    {
        try
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (id != userId)
            {
                throw new Exception("UnAuthorized Access");
            }
            var product = await _productService.Delete(id);
            return Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }  
}