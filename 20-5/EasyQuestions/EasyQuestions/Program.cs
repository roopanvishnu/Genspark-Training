using System;
using System.Collections.Generic;

class Employee
{
    int id, age;
    string name;
    double salary;

    public Employee()
    {
    }

    public Employee(int id, int age, string name, double salary)
    {
        this.id = id;
        this.age = age;
        this.name = name;
        this.salary = salary;
    }

    public void TakePromotionOrder()
    {
        List<string> promotionList = new List<string>();

        Console.WriteLine("Enter employee names in the order of their eligibility for promotion (press Enter to stop):");

        while (true)
        {
            Console.Write("Enter employee name: ");
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
                break;

            promotionList.Add(name);
        }

        Console.WriteLine($"\nThe current size of the collection is: {promotionList.Capacity}");

        promotionList.TrimExcess();  // Optimize memory usage

        Console.WriteLine($"The size after removing the extra space is: {promotionList.Capacity}");
    }

    public override string ToString()
    {
        return $"Employee ID: {id}\nName: {name}\nAge: {age}\nSalary: {salary}";
    }

    public int Id { get => id; set => id = value; }
    public int Age { get => age; set => age = value; }
    public string Name { get => name; set => name = value; }
    public double Salary { get => salary; set => salary = value; }
}

class EmployeePromotion
{
    public void TakePromotionOrder()
    {
        List<Employee> promotionList = new List<Employee>();

        Console.WriteLine("Enter employee details in promotion eligibility");

        while (true)
        {
            Console.WriteLine("\n--- New Employee ---");
            Employee emp = new Employee();

            Console.Write("Enter employee name (or press Enter to stop): ");
            string nameCheck = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nameCheck))
                break;

            emp.Name = nameCheck;

            Console.Write("Enter employee ID: ");
            emp.Id = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter employee age: ");
            emp.Age = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter employee salary: ");
            emp.Salary = Convert.ToDouble(Console.ReadLine());

            promotionList.Add(emp);
        }

        Console.WriteLine("\n--- Promotion Order ---");
        for (int i = 0; i < promotionList.Count; i++)
        {
            Console.WriteLine($"#{i + 1}");
            Console.WriteLine(promotionList[i]);
            Console.WriteLine("------------------------");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        EmployeePromotion promotion = new EmployeePromotion();
        promotion.TakePromotionOrder();
    }
}
