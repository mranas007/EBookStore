using BookShopingCartMVC.Services.IServices;

namespace BookShopingCartMVC.Services
{
    public class FileServices : IFileServices
    {
        private readonly IWebHostEnvironment _environment;
        public FileServices(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string[] allowedExtensions)
        {
            var wwwPath = _environment.WebRootPath;
            var fullPath = Path.Combine(wwwPath, "Uploads");
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            var extension = Path.GetExtension(file.FileName);
            if (allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException($"Only {string.Join(",", allowedExtensions)} files allowed.");
            }
            string fileName = Guid.NewGuid() + extension;
            string fileNameWithPath = Path.Combine(fullPath, fileName);
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;
        }

        public void DeteleFile(string fileName)
        {
            var wwwPath = _environment.WebRootPath;
            var fullPath = Path.Combine(wwwPath, "Uploads", fileName);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException(fileName);
            File.Delete(fullPath);
        }
    }
}
