using HotelOrderApp.Interfaces;

namespace HotelOrderApp.Models;

public class Order
{
    public IFood FoodItem { get; set; }
    public DateTime OrderAt { get; set; }
}