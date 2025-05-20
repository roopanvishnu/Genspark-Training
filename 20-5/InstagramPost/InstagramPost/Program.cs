using System;

class Program
{
    static void CuriousOnJaggedArray()
    {
        int[][] JaggedArray = new int[3][];
        JaggedArray[0] = new int[] { 1, 2, 3 };
        JaggedArray[1] = new int[] { 4, 5 };
        JaggedArray[2] = new int[] { 6 }; // Add third row to match declared size

        Console.WriteLine("Displaying Jagged Array:");
        for (int i = 0; i < JaggedArray.Length; i++)
        {
            for(int j = 0;j< JaggedArray[i].Length; j++)
            {
                Console.WriteLine(JaggedArray[i][j]);
            }
        }
    }

    static void CollectingInstagramData(out string[][][] userPosts)
    {
        Console.WriteLine("Enter the number of users ");
        int userCount = int.Parse(Console.ReadLine());
     
        userPosts = new string[userCount][][];
        for (int i = 0;i < userCount; i++)
        {
            Console.Write($"user {i + 1}: how many posts: ");
            int postCount = int.Parse(Console.ReadLine());

            userPosts[i] = new string[postCount][];
            for(int j =  0; j < postCount;j++)
            {
                userPosts[i][j] = new string[2];
                Console.Write($"Enter the caption for the post {j + 1}: ");
                userPosts[i][j][0] = Console.ReadLine();

                Console.Write("Enter Likes ");
                userPosts[i][j][1] = Console.ReadLine();
            }
        }
    }
    static void DisplayInstagramPosts(string[][][] usersPosts)
    {
        Console.WriteLine("\n Dsiplaying the Instagram Posts");

        for(int i = 0; i<usersPosts.Length;i++)
        {
            Console.WriteLine($"User {i + 1}");
            for(int j = 0; j < usersPosts[i].Length;j++)
            {
                string caption = usersPosts[i][j][0];  
                int likes = int.Parse(usersPosts[i][j][1]);

                Console.WriteLine($"Posts {j + 1} Caption: {caption} |Likes: {likes}");
            }
            Console.WriteLine();
        }
    }
    static void Main(string[] args)
    {
        CuriousOnJaggedArray();
        string[][][] posts;
        CollectingInstagramData(out posts);
        DisplayInstagramPosts(posts);
    }
}