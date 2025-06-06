using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Notify.Misc;
using Notify.Models;
using Notify.Services;

namespace Notify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    // [CustomExceptionFilter]
    public class DocumentController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DocumentService _documentService;
        private readonly IHubContext<NotificationHub> _notificationHub;
        public DocumentController(UserService userService, DocumentService documenService, IHubContext<NotificationHub> hubContext)
        {
            _userService = userService;
            _documentService = documenService;
            _notificationHub = hubContext;
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<ActionResult> Upload(IFormFile formFile)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (User == null || email == null) return Unauthorized("User not Authenticated");
                var user = await _userService.GetUserByEmail(email);
                DateTime currentTime = DateTime.Now.ToUniversalTime();
                Document doc = new Document
                {
                    Id = $"{user.Id}_{currentTime.Ticks}",
                    OriginalFileName = formFile.FileName,
                    UploadedBy = user.Id,
                    UploadedAt = currentTime.ToUniversalTime()
                };
                doc = await _documentService.AddDocument(doc);

                string ext = formFile.FileName.Split(".").LastOrDefault() ?? "txt";
                string path = $"/Users/roopanvishnu/Downloads/NotifyDocs/{doc.Id}.{ext}";
                using (var stream = System.IO.File.Create(path))
                {
                    await formFile.CopyToAsync(stream);
                }
                try
                {
                    await _notificationHub.Clients.All.SendAsync("RecieveMessage",user.Name, $"Added {doc.Id}.{ext}");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }

                return Ok($"Document uploaded and saved at {path}");
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid Request\n" + ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<Document>>> ViewAll()
        {
            var docs = await _documentService.GetAll();
            return docs.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetDocument(string id)
        {
            Document doc = await _documentService.GetDocument(id);
            // Regex regex = new Regex(@$"{id}.*");
            var file = Directory.GetFiles("/Users/hayagreevanv/Downloads/NotifyDocs", $"{id}.*").FirstOrDefault();
            // System.Console.WriteLine(file);
            if (file == null) throw new Exception("No document found");
            // string path = $"./Documents/{id}";
            return File(new FileStream(file, FileMode.Open), "application/octet-stream", doc.OriginalFileName);
        }
    }
}
