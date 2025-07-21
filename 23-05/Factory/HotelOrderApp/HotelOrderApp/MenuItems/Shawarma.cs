using HotelOrderApp.Interfaces;

namespace HotelOrderApp.MenuItems;

public class Shawarma : IFood
{
    public string Name => "Shawarma";
    public double Price => 80.00;
}