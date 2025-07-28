using ChienVHShopAPI.DTOs;
using ChienVHShopAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

// latest

namespace ChienVHShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto dto)
        {
            var created = await _service.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderUpdateDto dto)
        {
            var updated = await _service.UpdateOrderAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteOrderAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
