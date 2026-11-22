namespace DemoAPINET.DTOs
{
    public class FileSettingsOptions
    {
        public long MaxImageSize { get; set; }
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
        public string[] AllowedContentTypes { get; set; } = Array.Empty<string>();
    }
}