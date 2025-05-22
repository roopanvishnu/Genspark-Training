//Objects of a derived class should be able to replace objects of the base class
//without breaking the program

//bad example
//using System;

//namespace LSPBadExample
//{
//    public class Bird
//    {
//        public virtual void Fly()
//        {
//            Console.WriteLine("This bird can fly.");
//        }
//    }

//    public class Ostrich : Bird
//    {
//        public override void Fly()
//        {
//            Console.WriteLine("Ostriches can't fly");
//        }
//    }

//    class Program
//    {
//        static void MakeBirdFly(Bird bird)
//        {
//            bird.Fly();
//        }

//        static void Main(string[] args)
//        {
//            Bird flyingBird = new Bird();
//            Bird ostrich = new Ostrich();

//            MakeBirdFly(flyingBird);
//            MakeBirdFly(ostrich);
//        }
//    }
//}


// crt version
using System;
namespace LSPGoodExample
{
    public class Bird
    {
        public string? Birdname {  get; set; }
    }
    public class FlyingBird:Bird
    {
        public virtual void Fly()
        {
            Console.WriteLine($"{Birdname} is flying");
        }
    }
    public class Sparrow:FlyingBird
    {
        public override  void Fly()
        {
            Console.WriteLine($"{Birdname} is flying");
        }
    }
    public class Ostrich: Bird
    {
        public void Run()
        {
            Console.WriteLine($"{Birdname} can run fast");
        }
    }

    class Program
    {
        static void MakeFlyingBirdFly(FlyingBird bird)
        {
            bird.Fly();
        }
        static void Main(string[] args)
        {
            Sparrow sparrow = new Sparrow { Birdname = "Sparrow" };
            Ostrich ostrich = new Ostrich { Birdname = "Ostrich" };

            MakeFlyingBirdFly(sparrow);
            ostrich.Run();
        }
    }
}