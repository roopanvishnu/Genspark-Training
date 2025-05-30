using BankChatApplication.Interfaces;
using BankChatApplication.Models;

namespace BankChatApplication.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<ChatResponse> GetAnswerAsync(string prompt)
    {
        var answer = await _chatRepository.AskChatGptAsync(prompt);
        return new ChatResponse { Answer = answer };
    }

    public async Task<List<string>> GetPredefinedQuestionsAsync()
    {
        var questions = new List<string>
        {
            "What is a fixed deposit? in 2 lines",
            "How does net banking work?",
            "What are the benefits of a savings account? in 2 lines",
            "How to apply for a credit card? 2 lines",
            "What is an overdraft facility? 2 lines",
            "How does mobile banking differ from net banking? 2 lines",
            "What are the latest interest rates for savings accounts? 2 lines",
            "How can I block my lost debit card? 2 lines",
            "What is the minimum balance for a current account? 2 lines",
            "How does UPI work in banking? 2 lines"
        };

        return await Task.FromResult(questions);
    }
}
