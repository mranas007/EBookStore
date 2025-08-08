namespace BookShopingCartMVC.Services.IServices
{
    public interface IPdfGenerator
    {
        byte[] GeneratePdfForOrderList(Order orderDetails);
    }
}
