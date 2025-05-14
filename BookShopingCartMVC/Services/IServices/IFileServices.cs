namespace BookShopingCartMVC.Services.IServices
{
    public interface IFileServices
    {
        Task<string> SaveFileAsync(IFormFile file, string[] allowedExtensions);
        void DeteleFile(string fileName);
    }
}