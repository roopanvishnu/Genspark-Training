// OrderCreateDto.cs
namespace ChienVHShopAPI.DTOs
{
    public class OrderCreateDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
