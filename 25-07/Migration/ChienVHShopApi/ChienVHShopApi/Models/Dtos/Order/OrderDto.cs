// OrderDto.cs
namespace ChienVHShopAPI.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
