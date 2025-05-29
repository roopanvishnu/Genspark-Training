namespace BankingApp.DTOs
{
    public class CreateAccountDto
    {
        public int BranchId { get; set; }
        public List<int> OwnerIds { get; set; } = new List<int>();
        public decimal InitialBalance { get; set; }
    }

    public class AccountDto
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public int BranchId { get; set; }
        public List<CustomerDto> Owners { get; set; } = new List<CustomerDto>();
    }
}