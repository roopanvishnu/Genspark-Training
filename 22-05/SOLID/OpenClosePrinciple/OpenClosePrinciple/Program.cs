// Open/Closed Principle (OCP)

// definition => {A class should be open for extension but closed for modification}
// we should be able to add new functionality without changing the existing code. 

//bad example
//public class DiscountCalculator
//{
//    public double CalculateDiscount(string CustomerType, double amount)
//    {
//        if (CustomerType == "Regular")
//        {
//            return amount * 0.1;
//        }
//        else if (CustomerType == "Premium")
//        {
//            return amount * 0.2;
//        }
//        else if (CustomerType == "Gold")
//        {
//            return amount * 0.4;
//        }
//        else
//            return 0;
//    }
//}
//class Program
//{
//    static void Main(string[] args)
//    {
//        DiscountCalculator cd = new DiscountCalculator();
//        var dis = cd.CalculateDiscount("Gold", 100.00);
//        Console.WriteLine($"Discounted amount is {dis}");
//    }
//}

// good example 

public interface IDiscountStrategy
{
    double CalculateDiscount(double amount);
}
public class RegularCustomer : IDiscountStrategy
{
    public double CalculateDiscount(double amount) => amount * 0.1;
}
public class PremiumCustomer : IDiscountStrategy
{
    public double CalculateDiscount(double amount) => amount * 0.2;
}
public class GoldCustomer : IDiscountStrategy
{
    public double CalculateDiscount(double amount) => amount * 0.4;
}
public class PlatinumCustomer : IDiscountStrategy
{
    public double CalculateDiscount(double amount) => amount * 0.5;
}
public class DiscountCalculator
{
    public double GetDiscount(IDiscountStrategy customerType, double amount)
    {
        return customerType.CalculateDiscount(amount);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var calculator = new DiscountCalculator();
        IDiscountStrategy customer = new PlatinumCustomer();
        double discount = calculator.GetDiscount(customer, 1000);

        Console.WriteLine($"Dicounted amount is: {discount}");
    }
}