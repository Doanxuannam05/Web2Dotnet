// Services/FileService.cs
using DemoAPINET.DTOs;
using DemoAPINET.Models;
using DemoAPINET.Repositories;
using Microsoft.Extensions.Options;
using static DemoAPINET.DTOs.FileUploadDbDto;

namespace DemoAPINET.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly FileSettingsOptions _fileSettings;

        public FileService(IFileRepository fileRepository, IOptions<FileSettingsOptions> fileOptions)
        {
            _fileRepository = fileRepository;
            _fileSettings = fileOptions.Value;
        }

        public async Task<FileResponseDto> UploadFileAsync(UploadFileDto dto)
        {
            var file = dto.File;

            // 1. Kiểm tra file rỗng (Bài 1)
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Vui lòng chọn file hợp lệ (kích thước lớn hơn 0).");
            }

            // 2. Validate định dạng và dung lượng qua Options Pattern (Bài 2 & Bài 3)
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_fileSettings.AllowedExtensions.Contains(extension) ||
                !_fileSettings.AllowedContentTypes.Contains(file.ContentType))
            {
                throw new ArgumentException("Định dạng file không được hỗ trợ.");
            }

            if (file.Length > _fileSettings.MaxImageSize)
            {
                throw new ArgumentException($"Dung lượng file vượt quá giới hạn ({_fileSettings.MaxImageSize / (1024 * 1024)}MB).");
            }

            // 3. Đọc dữ liệu nhị phân vào mảng byte[]
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // 4. Tạo Entity StoredFile
            var storedFile = new StoredFile
            {
                FileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType,
                Content = fileBytes,
                Length = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            var savedFile = await _fileRepository.AddAsync(storedFile);

            return new FileResponseDto
            {
                Id = savedFile.Id,
                FileName = savedFile.FileName,
                Size = savedFile.Length,
                UploadedAt = savedFile.UploadedAt
            };
        }

        public async Task<FileDownloadDto?> GetFileForDownloadAsync(int id)
        {
            var storedFile = await _fileRepository.GetByIdAsync(id);
            if (storedFile == null) return null;

            return new FileDownloadDto
            {
                FileName = storedFile.FileName,
                ContentType = storedFile.ContentType,
                Content = storedFile.Content
            };
        }

        public async Task<IEnumerable<FileResponseDto>> GetAllFilesAsync()
        {
            var files = await _fileRepository.GetAllAsync();
            return files.Select(f => new FileResponseDto
            {
                Id = f.Id,
                FileName = f.FileName,
                Size = f.Length,
                UploadedAt = f.UploadedAt
            });
        }
    }
}