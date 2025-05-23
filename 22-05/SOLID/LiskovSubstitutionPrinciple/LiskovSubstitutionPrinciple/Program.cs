//Objects of a derived class should be able to replace objects of the base class
//without breaking the program

//bad example
//using System;

//namespace LSPBadExample
//{
//    public class Bird
//    {
//        public string Name { get; set; }

//        public Bird(string name)
//        {
//            Name = name;
//        }

//        public virtual void Fly()
//        {
//            Console.WriteLine($"{Name} is flying.");
//        }
//    }

//    public class Ostrich : Bird
//    {
//        public Ostrich(string name) : base(name) { }

//        public override void Fly()
//        {
//            // Violates LSP: Ostriches can't fly, yet we override Fly method!
//            throw new InvalidOperationException("Ostriches can't fly!");
//        }
//    }

//    class Program
//    {
//        static void MakeBirdFly(Bird bird)
//        {
//            bird.Fly(); // Will crash if passed an Ostrich!
//        }

//        static void Main(string[] args)
//        {
//            Bird sparrow = new Bird("Sparrow");
//            Bird ostrich = new Ostrich("Ostrich");

//            MakeBirdFly(sparrow); // OK
//            MakeBirdFly(ostrich); // Runtime exception
//        }
//    }
//}



// crt version
using System;
using System.Collections.Generic;

namespace LSPGoodExample
{
    //  Base class only contains common attributes, not behaviors
    public abstract class Bird
    {
        public string Name { get; set; }

        protected Bird(string name)
        {
            Name = name;
        }
    }

    //  Interface for birds that can fly
    public interface IFlyable
    {
        void Fly();
    }

    //  Interface for birds that can run
    public interface IRunnable
    {
        void Run();
    }

    //  Flying bird: Sparrow
    public class Sparrow : Bird, IFlyable
    {
        public Sparrow(string name) : base(name) { }

        public void Fly()
        {
            Console.WriteLine($"{Name} is flying.");
        }
    }

    //  Flying bird: Eagle
    public class Eagle : Bird, IFlyable
    {
        public Eagle(string name) : base(name) { }

        public void Fly()
        {
            Console.WriteLine($"{Name} is soaring through the sky.");
        }
    }

    //  Non-flying bird: Ostrich
    public class Ostrich : Bird, IRunnable
    {
        public Ostrich(string name) : base(name) { }

        public void Run()
        {
            Console.WriteLine($"{Name} is running swiftly.");
        }
    }

    class Program
    {
        static void MakeBirdsFly(IEnumerable<IFlyable> birds)
        {
            foreach (var bird in birds)
            {
                bird.Fly();
            }
        }

        static void MakeBirdsRun(IEnumerable<IRunnable> birds)
        {
            foreach (var bird in birds)
            {
                bird.Run();
            }
        }

        static void Main(string[] args)
        {
            List<IFlyable> flyingBirds = new()
            {
                new Sparrow("Sparrow"),
                new Eagle("Eagle")
            };

            List<IRunnable> runningBirds = new()
            {
                new Ostrich("Ostrich")
            };

            Console.WriteLine("Flying Birds:");
            MakeBirdsFly(flyingBirds);

            Console.WriteLine("\nRunning Birds:");
            MakeBirdsRun(runningBirds);
        }
    }
}
