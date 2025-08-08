namespace BookShopingCartMVC.Services.IServices
{
    public interface IPdfGenerator
    {
        byte[] GeneratePdfForOrderList(string htmlContent);
    }
}
