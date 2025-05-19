using System;

class Program
{
    static bool IsValidOperator(string op)
    {
        return op == "+"|| op == "-" || op == "*"|| op == "/";
    }
    static double Calculate (double num1 , double num2, string op)
    {
        switch (op)
        {
            case "+":
                return num1 + num2;
            case "-":
                return num1 - num2;
            case "*":  
                return num1 * num2; 
            case "/":
                if(num1 !=0)
                    return num1 / num2;
                else
                {
                    Console.WriteLine("Error: Division by zero");
                    return double.NaN;
                }
            default:
                Console.WriteLine("Invalid operator");
                return double.NaN;
        }
    }
    static void Main (string[] args)
    {
        Console.WriteLine("Enter the first number");
        double num1 = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Enter the second number");
        double num2 = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter aa valid operator (+,-,*,/): ");
        string? op = Console.ReadLine();

        if (IsValidOperator(op))
        {
            double result = Calculate(num1, num2, op);
            if(!double.IsNaN(result))
            {
                Console.WriteLine("Result " + result);
            }
        }
        else
        {
            Console.WriteLine("Invalid operator");
        }
    }
}