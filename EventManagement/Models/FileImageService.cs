namespace EventManagement.Models
{
    public interface IFileImageService
    {
        void DeleteFile(string fileName, string directory);
        string SaveFile(IFormFile file, string directory, string[] allowedExtensions);
    }
    public class FileImageService:IFileImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string SaveFile(IFormFile file, string directory, string[] allowedExtensions)
        {
            var wwwPath = _webHostEnvironment.WebRootPath;
            var path = Path.Combine(wwwPath, directory);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException($"Only {string.Join(",", allowedExtensions)} are allowed");
            }
            var newFileName = $"{Guid.NewGuid()}_{file.FileName}{extension}";
            var fullPath = Path.Combine(path, newFileName);
            using var fileStream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(fileStream);
            return newFileName;
        }
        public void DeleteFile(string fileName, string directory)
        {
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, directory, fileName);
            if (!Path.Exists(fullPath))
            {
                throw new FileNotFoundException($"File {fileName} does not exist.");
            }
            File.Delete(fullPath);
        }
    }
}
