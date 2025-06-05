namespace FirstAPI.Models
{
    public class ErrorObjectDTO
    {
        public int ErrorNumber { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string ErrorType { get; set; } = string.Empty;
    }
}