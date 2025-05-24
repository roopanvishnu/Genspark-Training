using HotelOrderApp.Models;

namespace HotelOrderApp.Interfaces;

public interface IOrderService
{
    void PlaceOfOrder(string foodtype);
    void ShowOrders();
}