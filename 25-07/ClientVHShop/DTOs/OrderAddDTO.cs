namespace ChienVHShopOnline.DTOs;

public class OrderAddDTO
{
    public System.DateTime? OrderDate { get; set; } = DateTime.UtcNow;
    public string PaymentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
}