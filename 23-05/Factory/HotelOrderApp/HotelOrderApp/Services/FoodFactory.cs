using HotelOrderApp.Interfaces;
using HotelOrderApp.MenuItems;
using System;


namespace HotelOrderApp.Services;

public class FoodFactory : IFoodFactory
{
    public IFood CreateFood(string foodType)
    {
        return foodType.ToLower() switch
        {
            "biriyani" => new Biriyani(),
            "friedrice" => new FriedRice(),
            "shawarma" => new Shawarma(),

            _ => throw new ArgumentException("unknow food type")
        };
    }
}