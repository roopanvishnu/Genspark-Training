using System.Net;
using System.Net.Http.Json;
using ChienVHShopAPI.Dtos;
using ChienVHShopAPI.Interfaces;
using ChienVHShopAPI.Models;
using ChienVHShopAPI.Contexts;
using Microsoft.Extensions.Configuration;

namespace ChienVHShopAPI.Services
{
    public class ContactUsService : IContactUsService
    {
        private readonly ChienVHShopDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ContactUsService(ChienVHShopDbContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<bool> SubmitContactFormAsync(ContactUsDto dto)
        {
            var secret = _configuration["Recaptcha:SecretKey"];
            var recaptchaUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={dto.RecaptchaResponse}";

            var response = await _httpClient.PostAsync(recaptchaUrl, null);
            var captchaResult = await response.Content.ReadFromJsonAsync<RecaptchaResponse>();

            if (captchaResult is null || !captchaResult.Success)
                return false;

            var contact = new ContactU
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Content = dto.Content
            };

            _context.ContactUs.Add(contact);
            await _context.SaveChangesAsync();
            return true;
        }

        private class RecaptchaResponse
        {
            public bool Success { get; set; }
            public List<string> ErrorCodes { get; set; } = new();
        }
    }
}