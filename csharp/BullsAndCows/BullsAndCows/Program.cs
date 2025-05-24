using System;

class Program
{
    static void CheckBullsAndCows(string secret, string guess, out int bulls, out int cows)
    {
        bulls = 0;
        cows = 0;
        bool[] secretMatched = new bool[4];
        bool[] guessMatched = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            if (secret[i] == guess[i])
            {
                bulls++;
                secretMatched[i] = true;
                guessMatched[i] = true;
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (!guessMatched[i])
            {
                for (int j = 0; j < 4; j++)
                {
                    if (!secretMatched[j] && guess[i] == secret[j])
                    {
                        cows++;
                        secretMatched[j] = true;
                        break;
                    }
                }
            }
        }
    }

    static void Main(string[] args)
    {
        string secretWord = "GAME";
        int attempts = 0;
        bool isCorrect = false;

        Console.WriteLine("Welcome to Bulls and Cows!");
        Console.WriteLine("Guess the 4-letter secret word.");

        while (!isCorrect)
        {
            Console.Write("Enter your 4-letter guess: ");
            string guess = Console.ReadLine().ToUpper();

            if (guess.Length != 4)
            {
                Console.WriteLine("Please enter exactly 4 letters.");
                continue;
            }

            attempts++;
            CheckBullsAndCows(secretWord, guess, out int bulls, out int cows);

            Console.WriteLine($"{bulls} Bulls, {cows} Cows");

            if (bulls == 4)
            {
                isCorrect = true;
                Console.WriteLine($"Congratulations! You guessed it in {attempts} attempt(s).");
            }
        }
    }
}
