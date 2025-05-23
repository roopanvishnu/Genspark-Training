using SecureFileAccessApp.Models;
using SecureFileAccessApp.Services;
using SecureFileAccessApp.Interfaces;
using System;


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the user name");
        string userName = Console.ReadLine();

        Console.WriteLine("Enter role (Admin/User/Guest)");
        string role = Console.ReadLine();

        User user = new User(userName, role);
        IFile file = new ProxyFile(user);

        Console.WriteLine($"\nUser: {user.UserName} | Role: {user.Role}");
        file.Read();

        Console.WriteLine("\npress any key to exit");
        Console.ReadKey();
    }
}