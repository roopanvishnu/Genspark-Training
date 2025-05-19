using System;

class Program
{
    static void MergeArray()
    {
        int[] arr = { 1, 3, 5 };
        int[] arr2 = { 2, 4, 6 };

        int[] arr3 = new int[arr.Length + arr2.Length];
        
        for(int i = 0;i< arr.Length;i++)
        {
            arr3[i] = arr[i];
        }
        for(int i = 0;i< arr2.Length;i++)
        {
            arr3[i + arr.Length] = arr2[i];
        }
        Console.WriteLine(arr3.ToString()); //i wrote
        Console.WriteLine(string.Join(", ", arr3)); // reference
    }
    static void Main(string[] args)
    {
        MergeArray();
    }
}