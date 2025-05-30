using BankChatApplication.Models;

namespace BankChatApplication.Interfaces;

public interface IChatService
{
    Task<ChatResponse> GetAnswerAsync(string prompt);
    Task<List<string>> GetPredefinedQuestionsAsync();
}

