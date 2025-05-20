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
        Console.Write("Enter ID: ");
        Id = int.Parse(Console.ReadLine());
        Console.Write("Enter Name: ");
        Name = Console.ReadLine();
        Console.Write("Enter Age: ");
        Age = int.Parse(Console.ReadLine());
        Console.Write("Enter Salary: ");
        Salary = double.Parse(Console.ReadLine());
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
    static Dictionary<int, Employee> employees = new Dictionary<int, Employee>();
    static void Main(string[] args)
    {
        bool running = true;
        while(running)
        {
            Console.WriteLine("\n--- Employee Management Menu ---");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Display All Employees");
            Console.WriteLine("3. Modify Employee Details");
            Console.WriteLine("4. Display Employee by ID");
            Console.WriteLine("5. Delete Employee");
            Console.WriteLine("6. Sort Employees by Salary");
            Console.WriteLine("7. Find Employee(s) by Name");
            Console.WriteLine("8. Exit");
            Console.Write("Choose an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    AddEmployee();
                    break;
                case "2":
                    DisplayAllEmployees();
                    break;
                case "3":
                    ModifyEmployee();
                    break;
                case "4":
                    DisplayEmployeeById();
                    break;
                case "5":
                    DeleteEmployee();
                    break;
                case "6":
                    SortEmployeesBySalary();
                    break;
                case "7":
                    FindEmployeesByName();
                    break;
                case "8":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    static void AddEmployee()
    {
        Employee emp = new Employee();
        emp.TakeEmployeeDetailsFromUser();
        if(!employees.ContainsKey(emp.Id))
        {
            employees.Add(emp.Id, emp);
            Console.WriteLine("Employee added ");
        }
        else
        {
            Console.WriteLine("Employee with this Id already exists");
        }
    }
    static void DisplayAllEmployees()
    {
        if(employees.Count == 0)
        {
            Console.WriteLine("No Employees to display");
            return;
        }
        foreach (var emp in employees.Values)
        {
            Console.WriteLine(emp);
        }
    }
    static void ModifyEmployee()
    {
        Console.Write("Enter the id of the emplyee to modify: ");
        int id = int.Parse(Console.ReadLine());

        if(employees.TryGetValue(id,out Employee emp))
        {
            Console.Write("Enter new name: ");
            emp.Name = Console.ReadLine();
            Console.Write("Enter new age: ");
            emp.Age = int.Parse(Console.ReadLine());
            Console.Write("Enter new salary: ");
            emp.Salary = double.Parse(Console.ReadLine());
            Console.WriteLine("Employee details updated.");
        }
        else
        {
            Console.WriteLine("Employee found dead");
        }
    }
    static void DisplayEmployeeById()
    {
        Console.Write("Enter the ID to search ");
        int id = int.Parse(Console.ReadLine());

        if (employees.TryGetValue(id,out Employee emp))
        {
            Console.WriteLine(emp);
        }
        else
        {
            Console.WriteLine("EMployee found dead");
        }
    }
    static void DeleteEmployee()
    {
        Console.Write("Enter the ID to delete: ");
        int id = int.Parse(Console.ReadLine());

        if (employees.Remove(id))
        {
            Console.WriteLine("Employee removed");
        }
        else
        {
            Console.WriteLine("EMplyee not found");
        }
    }
    static void SortEmployeesBySalary()
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("No employees to sort.");
            return;
        }

        var sortedList = employees.Values.OrderBy(e => e.Salary).ToList();
        foreach (var emp in sortedList)
        {
            Console.WriteLine(emp);
        }
    }

    static void FindEmployeesByName()
    {
        Console.Write("Enter the name to search: ");
        string name = Console.ReadLine();

        var matches = employees.Values.Where(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

        if (matches.Count > 0)
        {
            foreach (var emp in matches)
            {
                Console.WriteLine(emp);
            }
        }
        else
        {
            Console.WriteLine("No employee found with the given name.");
        }
    }
}