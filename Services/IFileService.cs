// Services/IFileService.cs
using DemoAPINET.DTOs;
using static DemoAPINET.DTOs.FileUploadDbDto;

namespace DemoAPINET.Services
{
    public interface IFileService
    {
        Task<FileResponseDto> UploadFileAsync(UploadFileDto dto);
        Task<FileDownloadDto?> GetFileForDownloadAsync(int id);
        Task<IEnumerable<FileResponseDto>> GetAllFilesAsync();
    }
}