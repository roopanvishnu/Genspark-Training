using System;
using System.Diagnostics;

class Program
{
    // static void GreetUser(string name)
    // {
    //     if(!string.IsNullOrEmpty(name))
    //     {
    //         Console.WriteLine("Hello "+ name);
    //     }
    //     else
    //     {
    //         Console.WriteLine("Enter a valid name");
    //     }
    // }
    static int LargestNumber(int num1,int num2)
    {
        if (num1 > num2)
        {
            return num1;
        }
        else
        {
            return num2;
        }
    }
    
    static void Main(string[] args)
    {
        // Console.WriteLine("Enter your Name");
        // string name = Console.ReadLine();

        // GreetUser(name);

        Console.WriteLine("Enter the first number ");
        int num1 = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Enter the Second Number");
        int num2 = Convert.ToInt32(Console.ReadLine());

        int Largest = LargestNumber(num1,num2);
        Console.WriteLine($"The largest number is {Largest}");
    }
}
