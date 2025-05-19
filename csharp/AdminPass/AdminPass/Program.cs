using System;

class Program
{
    static void Login()
    {
        const string correctUserName = "Admin";
        const string correctPassword = "pass";
        int attempts = 0;
        bool isAuthenticated = false;

        while (attempts < 3)
        {
            Console.WriteLine("Enter Username");
            string username = Console.ReadLine();

            Console.WriteLine("Enter password");
            string passcode = Console.ReadLine();  

            if (username == correctUserName && passcode == correctPassword)
            {
                Console.WriteLine("Login done successfully as Admin");
                isAuthenticated = true; ;
                break;
            }
            else
            {
                attempts++;
                Console.WriteLine("Incorrect password.Attempts left: " + (3 - attempts));
            }
        }
        if (!isAuthenticated)
        {
            Console.WriteLine("Invalid attemsts for 3 times exiting....");
        }
    }
    static void Main(string[] args)
    {
        Login();
    }
}