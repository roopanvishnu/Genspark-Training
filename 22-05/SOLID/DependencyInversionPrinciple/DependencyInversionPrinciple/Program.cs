// dependency inversion principle
// in simple words 

// high level modules should not depend on low level modules both should depend on abstraction
// bad example
//using System;

//namespace DIPBadexample
//{
//    public class EmailNotification
//    {
//        public void Send(string message)
//        {
//            Console.WriteLine($"sending email {message}");
//        }
//    }
//    class Program
//    {
//        // this one has tight cou[le
//        static void Main(string[] args)
//        {
//            EmailNotification notifier = new EmailNotification();
//            notifier.Send("Welcome to our app!");

//        }
//    }
//}

// good example
using System;
namespace DPIGoodExample
{
    public interface INotificationService
    {
        void Send (string message);
    }
    public class EmailNotification: INotificationService
    {
        public void Send(string message)
        {
            Console.Write($"Sending Email: {message}");
        }
    }
    public class SMSNotification : INotificationService
    {
        public void Send(string message)
        {
            Console.WriteLine($"Sending SMS: {message}");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            INotificationService notifier = new EmailNotification();
            notifier.Send("Welcome to our app!");

            notifier = new SMSNotification();
            notifier.Send("OTP: 123456");
        }
    }
}