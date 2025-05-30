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
            "What is a fixed deposit?",
            "How does net banking work?",
            "What are the benefits of a savings account?",
            "How to apply for a credit card?",
            "What is an overdraft facility?",
            "How does mobile banking differ from net banking?",
            "What are the latest interest rates for savings accounts?",
            "How can I block my lost debit card?",
            "What is the minimum balance for a current account?",
            "How does UPI work in banking?"
        };

        return await Task.FromResult(questions);
    }
}
