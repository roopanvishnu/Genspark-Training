namespace BankingApp.DTOs
{
    public class CreateCustomerDto
    {
        public string FullName { get; set; } = string.Empty;
    }

    public class CustomerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}