using System;
using System.Collections.Generic;
using System.Linq;

class Employee
{
    public int Id { get; set; }
    public int Age { get; set; } 
    public string Name { get; set; }
    public double Salary { get; set; }

    public Employee() { }

    public Employee(int id, int age, string name, double salary)
    {
        Id = id;
        Age = age;
        Name = name;
        Salary = salary;
    }

    public void TakeEmployeeDetailsFromUser()
    {
        Console.Write("Enter Employee ID: ");
        Id = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Employee Name: ");
        Name = Console.ReadLine();

        Console.Write("Enter Employee Age: ");
        Age = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Employee Salary: ");
        Salary = Convert.ToDouble(Console.ReadLine());
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Age: {Age}, Salary: {Salary}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Employee> employeeList = new List<Employee>();

        Console.Write("Enter number of employees to add: ");
        int count = int.Parse(Console.ReadLine());

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"\nEnter details for Employee {i + 1}:");
            Employee emp = new Employee();
            emp.TakeEmployeeDetailsFromUser();
            employeeList.Add(emp);
        }

        Console.WriteLine("enter the name of the employee searching for?");
        string searchName = Console.ReadLine();

        var matchedName = employeeList.Where(e=>e.Name.Equals(searchName,StringComparison.OrdinalIgnoreCase)).ToList();
        if (matchedName.Count > 0 )
        {
            Console.WriteLine($"Employees with name {searchName}: ");
            foreach( var emp in matchedName)
            {
                Console.WriteLine(emp);
            }
        }
        else
        {
            Console.WriteLine("Employee found dead!");
        }
    }
}
