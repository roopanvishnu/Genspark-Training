using BankChatApplication.Interfaces;
using BankChatApplication.Models;

namespace BankChatApplication.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request)
    {
        var response = await _chatService.GetAnswerAsync(request.Prompt);
        return Ok(response);
    }

    [HttpGet("questions")]
    public async Task<IActionResult> GetPredefinedQuestions()
    {
        var questions = await _chatService.GetPredefinedQuestionsAsync();
        return Ok(questions);
    }
}
