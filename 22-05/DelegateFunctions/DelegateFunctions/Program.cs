using WholeApplication.Misc;

namespace WholeApplication
{
    class Employee
    {
        public int Id { get; }
        public int Age { get; }
        public string Name { get; }
        public decimal Salary { get; }

        public Employee(int id, int age, string name, decimal salary)
        {
            Id = id;
            Age = age;
            Name = name;
            Salary = salary;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Age: {Age}, Salary: {Salary}";
        }
    }

    class Program
    {
        List<Employee> employees = new List<Employee>()
        {
            new Employee(101,30, "John Doe",  50000),
            new Employee(102, 25,"Jane Smith",  60000),
            new Employee(103,35, "Sam Brown",  70000)
        };
        //public delegate void MyDelegate<T>(T num1, T num2);
        //public delegate void MyFDelegate(float num1, float num2);
        public void Add(int n1, int n2)
        {
            int sum = n1 + n2;
            Console.WriteLine($"The sum of {n1} and {n2} is {sum}");
        }
        public void Product(int n1, int n2)
        {
            int prod = n1 * n2;
            Console.WriteLine($"The sum of {n1} and {n2} is {prod}");
        }
        Program()
        {
            //MyDelegate<int> del = new MyDelegate<int>(Add);
            Action<int, int> del = Add;
            del += Product;
            //del += delegate (int n1, int n2)
            //{
            //    Console.WriteLine($"The division result of {n1} and {n2} is {n1 / n2}");
            //};
            del += (int n1, int n2) => Console.WriteLine($"The division result of {n1} and {n2} is {n1 / n2}");
            del(100, 20);
        }
        void FindEmployee()
        {
            int empId = 102;
            Predicate<Employee> predicate = e => e.Id == empId;
            Employee? emp = employees.Find(predicate);
            Console.WriteLine(emp.ToString() ?? "No such employee");
        }
        void SortEmployee()
        {
            var sortedEmployees = employees.OrderBy(e => e.Name);
            foreach (var emp in sortedEmployees)
            {
                Console.WriteLine(emp.ToString());
            }
        }
        static void Main(string[] args)
        {
            
            Program program = new();
            program.FindEmployee();
            program.SortEmployee();
            string str = "Studen";
            Console.WriteLine(str.StringValidationCheck());
        }
    }
}

//namespace WholeApplication
//{
//    internal class Program
//    {
//        public delegate void MyDelegate<T>(T num1, T num2);
//        public delegate void MyFDelegate(float num1, float num2);
//        public void Add(int n1, int n2)
//        {
//            int sum = n1 + n2;
//            Console.WriteLine($"The sum of {n1} and {n2} is {sum}");
//        }
//        public void Product(int n1, int n2)
//        {
//            int prod = n1 * n2;
//            Console.WriteLine($"The sum of {n1} and {n2} is {prod}");
//        }
//        Program()
//        {
//            //MyDelegate<int> del = new MyDelegate<int>(Add);
//            Action<int, int> del = Add;
//            del += Product;

//            del += (int n1, int n2) => Console.WriteLine($"The division result of {n1} and {n2} is {n1 / n2}");
//            del(100, 20);
//        }
//        static void Main(string[] args)
//        {

//            Program program = new();
//        }
//    }
//}

//namespace WholeApplication
//{
//    internal class Program
//    {
//        public delegate void MyDelegate(int num1, int num2);
//        public delegate void Greet(string name);
//        public static void SayHello(string name)
//        {
//            Console.WriteLine($"Hello {name}");
//        }

//        public void Add(int n1, int n2)
//        {
//            int sum = n1 + n2;
//            Console.WriteLine($"The sum of {n1} and {n2} is {sum}");
//        }
//        public void Product(int n1, int n2)
//        {
//            int prod = n1 * n2;
//            Console.WriteLine($"The sum of {n1} and {n2} is {prod}");
//        }
//        Program()
//        {
//            MyDelegate del = new MyDelegate(Add);
//            del += Product;
//            del(10, 20);
//        }
//        static void Main(string[] args)
//        {

//            new Program(); 
//            Greet g = SayHello;
//            g("Roopan");
//        }
//    }
//}
