namespace BankChatApplication.Interfaces;

public interface IChatRepository
{
    Task<string> AskChatGptAsync(string prompt);
}
