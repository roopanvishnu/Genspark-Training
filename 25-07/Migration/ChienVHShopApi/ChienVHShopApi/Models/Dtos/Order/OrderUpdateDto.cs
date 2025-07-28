// OrderUpdateDto.cs
namespace ChienVHShopAPI.DTOs
{
    public class OrderUpdateDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
