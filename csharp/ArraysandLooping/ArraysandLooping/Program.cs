using System;

class Program
{
    static void Main(string[] args)
    {
        int[] arr = { 1, 2, 2, 3, 4, 4, 4, 1};
        bool[] visited = new bool[arr.Length];

        for (int i = 0; i < arr.Length; i++)
        {
            if (visited[i])
                continue;

            int count = 1;
            for (int j = i + 1; j < arr.Length; j++)
            {
                if (arr[i] == arr[j])
                {
                    count++;
                    visited[j] = true;
                }
            }

            Console.WriteLine(arr[i] + " occurs " + count + " times");
        }
    }
}
