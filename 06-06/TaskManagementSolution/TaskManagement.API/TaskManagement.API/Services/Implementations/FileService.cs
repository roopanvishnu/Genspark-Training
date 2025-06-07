using Microsoft.AspNetCore.Http;
using TaskManagement.API.DTOs.Request.File;
using TaskManagement.API.DTOs.Response.Common;
using TaskManagement.API.DTOs.Response.Task;
using TaskManagement.API.Models.Entities;
using TaskManagement.API.Repositories.Interfaces;
using TaskManagement.API.Repositories.Interfaces.UnitOfWork;

namespace TaskManagement.API.Services.Implementations;

public class FileService : IFileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _storageService;
    private readonly INotificationService _notificationService;

    public FileService(IUnitOfWork unitOfWork, IFileStorageService storageService, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _notificationService = notificationService;
    }

    public async Task<ApiResponseDto<FileResponseDto>> UploadFileAsync(FileUploadRequestDto request, string uploadedBy)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId);
        if (task == null)
            return ApiResponseDto<FileResponseDto>.ErrorResponse("Task not found");

        var fileName = _storageService.GenerateUniqueFileName(request.File.FileName);
        var path = await _storageService.SaveFileAsync(request.File, fileName);

        var file = new FileAttachment
        {
            FileName = fileName,
            OriginalFileName = request.File.FileName,
            ContentType = request.File.ContentType,
            FileSizeBytes = request.File.Length,
            FilePath = path,
            TaskId = request.TaskId,
            UploadedByUserId = Guid.Parse(uploadedBy)
        };

        await _unitOfWork.FileAttachments.AddAsync(file);
        await _unitOfWork.SaveChangesAsync();

        await _notificationService.NotifyFileUploadedAsync(request.TaskId, fileName, uploadedBy);

        return ApiResponseDto<FileResponseDto>.SuccessResponse(MapToDto(file));
    }

    public async Task<ApiResponseDto<List<FileResponseDto>>> GetFilesByTaskIdAsync(Guid taskId)
    {
        var files = await _unitOfWork.FileAttachments.GetFilesByTaskIdAsync(taskId);
        var list = files.Select(MapToDto).ToList();
        return ApiResponseDto<List<FileResponseDto>>.SuccessResponse(list);
    }

    public async Task<ApiResponseDto<byte[]>> DownloadFileAsync(string fileName)
    {
        var file = await _unitOfWork.FileAttachments.GetByFileNameAsync(fileName);
        if (file == null)
            return ApiResponseDto<byte[]>.ErrorResponse("File not found");

        var bytes = await _storageService.GetFileAsync(fileName);
        return ApiResponseDto<byte[]>.SuccessResponse(bytes);
    }

    public async Task<ApiResponseDto<string>> DeleteFileAsync(Guid fileId, string deletedBy)
    {
        var file = await _unitOfWork.FileAttachments.GetByIdAsync(fileId);
        if (file == null)
            return ApiResponseDto<string>.ErrorResponse("File not found");

        await _storageService.DeleteFileAsync(file.FileName);
        await _unitOfWork.FileAttachments.DeleteAsync(file);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponseDto<string>.SuccessResponse("File deleted");
    }

    public async Task<ApiResponseDto<FileResponseDto>> GetFileByIdAsync(Guid fileId)
    {
        var file = await _unitOfWork.FileAttachments.GetByIdAsync(fileId);
        return file == null
            ? ApiResponseDto<FileResponseDto>.ErrorResponse("File not found")
            : ApiResponseDto<FileResponseDto>.SuccessResponse(MapToDto(file));
    }

    private static FileResponseDto MapToDto(FileAttachment file)
    {
        return new FileResponseDto
        {
            Id = file.Id,
            FileName = file.FileName,
            OriginalFileName = file.OriginalFileName,
            ContentType = file.ContentType,
            FileSizeBytes = file.FileSizeBytes,
            FilePath = file.FilePath,
            UploadedAt = file.UploadedAt
        };
    }
}
