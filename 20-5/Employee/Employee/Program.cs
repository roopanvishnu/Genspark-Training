
//medium

using System;
using System.Collections.Generic;

class Employee
{
    int id, age;
    string name;
    double salary;

    public Employee() { }

    public Employee(int id, int age, string name, double salary)
    {
        this.id = id;
        this.age = age;
        this.name = name;
        this.salary = salary;
    }

    public void TakeEmployeeDetailsFromUser()
    {
        Console.Write("Enter Employee ID: ");
        id = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Name: ");
        name = Console.ReadLine();

        Console.Write("Enter Age: ");
        age = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Salary: ");
        salary = Convert.ToDouble(Console.ReadLine());
    }

    public override string ToString()
    {
        return $"ID: {id}, Name: {name}, Age: {age}, Salary: {salary}";
    }

    public int Id { get => id; }
}

class Program
{
    static void Main(string[] args)
    {
        Dictionary<int, Employee> employeeDirectory = new Dictionary<int, Employee>();

        Console.Write("How many employees you want to enter? ");
        int count = int.Parse(Console.ReadLine());

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"\nEnter details for Employee {i + 1}");
            Employee emp = new Employee();
            emp.TakeEmployeeDetailsFromUser();

            if (!employeeDirectory.ContainsKey(emp.Id))
            {
                employeeDirectory.Add(emp.Id, emp);
            }
            else
            {
                Console.WriteLine("Employee ID already exists. Skipping entry.");
            }
        }

        Console.Write("\nEnter an Employee ID to fetch details: ");
        int searchId = int.Parse(Console.ReadLine());

        if (employeeDirectory.ContainsKey(searchId))
        {
            Console.WriteLine("\nEmployee Found:");
            Console.WriteLine(employeeDirectory[searchId]);
        }
        else
        {
            Console.WriteLine("Employee with the given ID not found.");
        }
    }
}
