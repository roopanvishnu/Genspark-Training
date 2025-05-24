using System;

class Program
{
    static void CountDivisible()
    {
        int count = 0;

        for(int i = 0; i < 10; i++)
        {
            Console.Write("Enter number: " + i + ": ");
            int number = Convert.ToInt32(Console.ReadLine());

            if (number % 7 == 0) ;
            {
                count++;    
            }
        }
        Console.WriteLine("Count of numbers divisible by 7: " + count);
    }
    static void Main(string[] args)
    {
        CountDivisible();
    }
}