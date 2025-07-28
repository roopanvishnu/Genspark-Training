using ChienVHShopAPI.Dtos;

namespace ChienVHShopAPI.Interfaces
{
    public interface IContactUsService
    {
        Task<bool> SubmitContactFormAsync(ContactUsDto dto);
    }
}