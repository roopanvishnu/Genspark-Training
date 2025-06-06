using TaskManagement.API.DTOs.Request.File;
using TaskManagement.API.DTOs.Response.Common;
using TaskManagement.API.DTOs.Response.Task;

namespace TaskManagement.API.Repositories.Interfaces;

public interface IFileService
{
    Task<ApiResponseDto<FileResponseDto>> UploadFileAsync(FileUploadRequestDto request, string uploadedBy);
    Task<ApiResponseDto<List<FileResponseDto>>> GetFilesByTaskIdAsync(Guid taskId);
    Task<ApiResponseDto<byte[]>> DownloadFileAsync(string fileName);
    Task<ApiResponseDto<string>> DeleteFileAsync(Guid fileId, string deletedBy);
    Task<ApiResponseDto<FileResponseDto>> GetFileByIdAsync(Guid fileId);
}