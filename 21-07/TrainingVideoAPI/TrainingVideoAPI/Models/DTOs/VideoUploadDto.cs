using Microsoft.AspNetCore.Http;

namespace TrainingVideoAPI.DTOs
{
    public class VideoUploadDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
    }
}
