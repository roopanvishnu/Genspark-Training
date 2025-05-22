// Interface Segregation Principle

//Don't put too many unrelated methods in one interface.
//Instead, split them into smaller, more specific interfaces.

//using System;

//namespace ISPBadExample
//{
//    public interface IWorker
//    {
//        void Work();
//        void Eat();
//    }
//    public class HumanWorker : IWorker
//    {
//        public void Work()
//        {
//            Console.WriteLine("Worker is working");
//        }
//        public void Eat()
//        {
//            Console.WriteLine("worker is eating");
//        }
//    }
//    public class RobotWorker : IWorker 
//    {
//        public void Work()
//        {
//            Console.WriteLine("Robot is working");
//        }
//        public void Eat()
//        {
//            Console.WriteLine("Robots don't eat but forced to use this method");
//        }
//    }
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            IWorker human = new HumanWorker();
//            human.Work();
//            human.Eat();

//            IWorker robot = new RobotWorker();  
//            robot.Work();
//            robot.Eat(); // this is not required 
//        }
//    }
//}

using System;
namespace ISPGoodExample
{
    public interface Iworker
    {
        void Work();
    }
    public interface IEating
    {
        void Eat();
    }
    public class HumanWorker:Iworker,IEating
    {
        public void Work()
        {
            Console.WriteLine("Human is working");
        }
        public void Eat()
        {
            Console.WriteLine("Human is eating");
        }
    }
    public class RobotWorker : Iworker
    {
        public void Work()
        { 
            Console.WriteLine("Robot is working");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var human = new HumanWorker();
            human.Work();
            human.Eat();

            var robot = new RobotWorker();
            robot.Work();
        }
    }
}