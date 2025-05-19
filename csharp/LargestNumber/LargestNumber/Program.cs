using System;

class Program
{
    static int LargestNumber(int num1, int num2)
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
    static void Main(string [] args)
    {
        Console.WriteLine("Enter the first number");
        int num1 = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Enter the second number");
        int num2 = Convert.ToInt32(Console.ReadLine());

        int largest = LargestNumber(num1, num2);
        Console.WriteLine("the largest number is " + largest);
    }
}