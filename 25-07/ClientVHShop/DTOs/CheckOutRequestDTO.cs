namespace ChienVHShopOnline.DTOs;

public class CheckOutRequestDTO
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public List<CartItemDTO> CartItems { get; set; } = new();

}