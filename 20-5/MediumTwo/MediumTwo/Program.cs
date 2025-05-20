using System;
using System.Collections.Generic;
using System.Linq;

class Employee: IComparable<Employee>
{
    int id;
    int age;
    string name;
    double salary;
    
    public Employee() { }
    public Employee(int id, int age,string name ,double salary)
    {
        this.id = id;
        this.age = age;
        this.name = name;
        this.salary = salary;
    }
    public void TakeEmployeeDetailsFromUser()
    {
        Console.Write("Enter Employee Id: ");
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
        return $"ID: {id},Name: {name}, Age: {age},Salary: {salary}";
    }
    public int Id => id;
    public double Salary => salary;
    public int CompareTo(Employee other)
    {
        return this.salary.CompareTo(other.salary);
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Employee> employees = new List<Employee>();

        Console.Write("How many employees do you wanna to enter? ");
        int count = int.Parse(Console.ReadLine());

        for(int i = 0; i < count;i++)
        {
            Console.WriteLine($"ENter the details for Employee {i + 1}");
            Employee employee = new Employee();
            employee.TakeEmployeeDetailsFromUser();
            employees.Add(employee);
        }
        employees.Sort();
        Console.WriteLine("Employees sorted by salary: ");
        foreach(var employee in employees)
        {
            Console.WriteLine(employee);
        }
        Console.Write("Enter the employee ID to search ");
        int searchId = int.Parse(Console.ReadLine());

        var result = employees.Where(e => e.Id == searchId).FirstOrDefault();
        if(result != null)
        {
            Console.WriteLine("Employee Found:");
            Console.WriteLine(result);
        }
        else
        {
            Console.WriteLine("Employee not found");
        }
    }
}