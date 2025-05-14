using BookShopingCartMVC.Services;

namespace BookShopingCartMVC.Areas.Admin.Controllers
{
    public class InvoiceController
    {
        private readonly InvoiceRenderingService _invoiceRenderingService;

        public InvoiceController(InvoiceRenderingService invoiceRenderingService)
        {
            _invoiceRenderingService = invoiceRenderingService;
        }
    }
}
