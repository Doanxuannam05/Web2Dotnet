using DemoAPINET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DemoAPINET.DTOs.FileUploadDbDto;

namespace DemoAPINET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesDbController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesDbController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // Upload file nhị phân vào Database (Yêu cầu đăng nhập JWT)
        [HttpPost("upload")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadFileDto dto)
        {
            var result = await _fileService.UploadFileAsync(dto);
            return CreatedAtAction(nameof(Download), new { id = result.Id }, result);
        }

        // Tải hoặc truy xuất file từ Database dựa trên ID
        [HttpGet("download/{id:int}")]
        public async Task<IActionResult> Download(int id)
        {
            var file = await _fileService.GetFileForDownloadAsync(id);
            if (file == null)
            {
                return NotFound(new { message = "Không tìm thấy file yêu cầu." });
            }

            return File(file.Content, file.ContentType, file.FileName);
        }

        // Lấy danh sách metadata của các file đã lưu
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var files = await _fileService.GetAllFilesAsync();
            return Ok(files);
        }
    }
}
