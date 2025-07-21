using System;

class Program
{
    static string EncryptMessage(string message)
    {
        string alphabet = "abcdefghijklmnopqrstuvwxyz";
        string result = "";

        foreach (char c in message)
        {
            int index = alphabet.IndexOf(c);
            if(index ==-1)
            {
                result += c;
            }
            else
            {
                int newIndex = (index + 3) % 26;
                result += alphabet[newIndex];
            }
        }
        return result;
    }
    static string DecryptMessage(string message)
    {
        string alphabet = "abcdefghijklmnopqrstuvwxyz";
        string result = "";

        foreach(char c in message)
        {
            int index = alphabet.IndexOf(c);
            if(index ==-1)
            {
                result += c;
            }
            else
            {
                int newIndex = (index - 3 )%26;
                result += alphabet[newIndex];
            }
        }
        return result;
    }
    static void Main(string[] args)
    {
        Console.Write("Enter a lowercase message ");
        string input = Console.ReadLine();  

        string encyrpted = EncryptMessage(input);
        Console.WriteLine("Encrypted: " + encyrpted);

        string decrypted = DecryptMessage(encyrpted);
        Console.WriteLine("Decrypted: " + decrypted);
    }
}