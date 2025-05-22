// single repository principle 

// so this means one class should have one core responsibility
//bad example
//public class Employee
//{
//    public int Id { get; set; } 
//    public string? Name { get; set; }    

//    public void CalculateSalary()
//    {
//        Console.WriteLine("Calculating salary");
//    }

//    public void SaveToDb()
//    {
//        Console.WriteLine("Saving to db");
//    }
//    public void GenerateReport()
//    {
//        Console.WriteLine("Generate Report");
//    }
//}
//class Program
//{
//    static void Main(string[] args)
//    {
//        Employee e = new Employee();
//        e.Name = "Roopan";
//        e.Id = 1;
//        e.CalculateSalary();
//        e.SaveToDb();
//        e.GenerateReport();
//    }
//}

//good example

public class Employee
{
    public int id {  get; set; }   
    public string name { get; set; }  
}
public class SalaryCalculator
{
    public void CalculateSalary(Employee e)
    {
        Console.WriteLine($"Calculating salary for {e.name}");
    }
}
public class EmployeeDb
{
    public void SaveDataDb(Employee e)
    {
        Console.WriteLine($"Saving {e.name} to db");
    }
}

public class GenerateReport
{
    public void GenerateReportForEmployee(Employee e)
    {
        Console.WriteLine($"Generate report for {e.name}");
    }
}
class Program
{
    static void Main(string[] args)
    {
        Employee e = new Employee();
        e.name = "Vishnu";
        e.id = 1;
        
        SalaryCalculator s = new SalaryCalculator();
        s.CalculateSalary(e);

        EmployeeDb ed = new EmployeeDb();
        ed.SaveDataDb(e);

        GenerateReport gr = new GenerateReport();
        gr.GenerateReportForEmployee(e);
    }
}