using System;

class Program
{
    static void RotateLeftByOne(int[] arr)
    {
        int first = arr[0]; 

        for (int i = 0; i < arr.Length - 1; i++)
        {
            arr[i] = arr[i + 1];
        }

        arr[arr.Length - 1] = first;  
    }

    static void Main(string[] args)
    {
        int[] arr = { 10, 20, 30, 40, 50 };

        Console.WriteLine("Original array:");
        foreach (int num in arr)
        {
            Console.Write(num + " ");
        }
        RotateLeftByOne(arr);

        Console.WriteLine("\nArray after left rotation by one position:");
        foreach (int num in arr)
        {
            Console.Write(num + "i ");
        }
    }
}
