namespace HotelOrderApp.Interfaces;

public interface IFoodFactory
{
    IFood CreateFood(string foodType);
}