using HotelOrderApp.Infrastructure;
using HotelOrderApp.Interfaces;
using HotelOrderApp.Services;
using System;

namespace HotelOrderApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IFoodFactory factory = new FoodFactory();
            IOrderService orderService = new OrderService(factory);

            while (true)
            {
                Console.WriteLine("\n--- Small Hotel Menu ---");
                Console.WriteLine("1. Biriyani");
                Console.WriteLine("2. FriedRice");
                Console.WriteLine("3. Shawarma");
                Console.WriteLine("4. Show Orders");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine()?.ToLower();
                switch (choice)
                {
                    case "1":
                    case "biriyani":
                        orderService.PlaceOfOrder("biriyani");
                        break;
                    case "2":
                    case "friedrice":
                        orderService.PlaceOfOrder("friedrice");
                        break;
                    case "3":
                    case "shawarma":
                        orderService.PlaceOfOrder("Shawarma");
                        break;
                    case "4":
                        orderService.ShowOrders();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}
