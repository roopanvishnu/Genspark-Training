using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BankChatApplication;
using BankChatApplication.Interfaces;
using Microsoft.Extensions.Options;

public class ChatRepository : IChatRepository
{
    private readonly HttpClient _httpClient;
    private readonly ApiContext _context;

    public ChatRepository(HttpClient httpClient, IOptions<ApiContext> contextOptions)
    {
        _httpClient = httpClient;
        _context = contextOptions.Value;
    }

    public async Task<string> AskChatGptAsync(string prompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_context.ApiKey}";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var request = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, request);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var reply = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return reply?.Trim() ?? "No response from Gemini.";
    }
}