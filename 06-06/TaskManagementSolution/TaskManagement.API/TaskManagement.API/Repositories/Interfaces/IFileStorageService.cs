namespace TaskManagement.API.Repositories.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string fileName);
    Task<byte[]> GetFileAsync(string fileName);
    Task<bool> DeleteFileAsync(string fileName);
    Task<bool> FileExistsAsync(string fileName);
    Task<long> GetFileSizeAsync(string fileName);
    string GenerateUniqueFileName(string originalFileName);
}