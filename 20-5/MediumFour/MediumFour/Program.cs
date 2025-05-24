using System;
using System.Collections.Generic;
using System.Linq;

class Employee : IComparable<Employee>
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

    public int CompareTo(Employee other)
    {
        return this.Salary.CompareTo(other.Salary);
    }
}

class Program
{
    static void Main()
    {
        Dictionary<int, Employee> employees = new Dictionary<int, Employee>();

        Console.Write("Enter number of employees to add: ");
        int count = Convert.ToInt32(Console.ReadLine());

        for (int i = 0; i < count; i++)
        {
            Employee emp = new Employee();
            emp.TakeEmployeeDetailsFromUser();

            if (!employees.ContainsKey(emp.Id))
                employees.Add(emp.Id, emp);
            else
                Console.WriteLine("Duplicate ID found. Skipping entry.");
        }

        Console.Write("\nEnter the ID of employee to compare by age: ");
        int targetId = Convert.ToInt32(Console.ReadLine());

        if (employees.ContainsKey(targetId))
        {
            int targetAge = employees[targetId].Age;

            var elderEmployees = employees.Values
                                          .Where(emp => emp.Age > targetAge)
                                          .ToList();

            if (elderEmployees.Count == 0)
            {
                Console.WriteLine("No employees are elder than the selected employee.");
            }
            else
            {
                Console.WriteLine("\nEmployees elder than the selected employee:");
                foreach (var emp in elderEmployees)
                {
                    Console.WriteLine(emp);
                }
            }
        }
        else
        {
            Console.WriteLine("Employee ID not found.");
        }
    }
}
