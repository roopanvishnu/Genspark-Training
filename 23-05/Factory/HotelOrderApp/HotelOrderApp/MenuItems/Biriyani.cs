using HotelOrderApp.Interfaces; 

namespace HotelOrderApp.MenuItems;

public class Biriyani : IFood
{
    public string Name => "Biriyani";
    public double Price => 249.90;
}