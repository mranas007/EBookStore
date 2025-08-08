using BookShopingCartMVC.Models.DTOs;
using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookShopingCartMVC.Repository
{
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserOrderRepository(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            var pranciple = _httpContextAccessor.HttpContext?.User;
            return _userManager.GetUserId(pranciple) ?? string.Empty;
        }

        public async Task<IEnumerable<Order>> UserOrderAsync(bool getAll = false)
        {
            var order = _context.Orders
                .Include(a => a.OrderStatus)
                .Include(a => a.OrderDetail)
                .ThenInclude(a => a.Book)
                .ThenInclude(a => a.Genre).AsQueryable();

            if (!getAll)
            {
                var userId = GetUserId();
                order = order.Where(a => a.UserId == userId);
                return await order.ToListAsync();
            }

            return await order.ToListAsync();
        }

        // chnage shiping order status
        public async Task ChangeOrderStatus(UpdateOrderStatusDto data)
        {
            var order = await _context.Orders.FindAsync(data.OrderId);
            if (order is null)
                throw new InvalidOperationException($"order with id: {data.OrderId} doesn't found!");

            order.OrderStatusId = data.OrderStatusId;
            await _context.SaveChangesAsync();
        }

        // change the payment paid or not
        public async Task TogglePaymentStatus(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order is null)
                throw new InvalidOperationException($"order with id: {orderId} doesn't found!");

            order.IsPaid = !order.IsPaid;
            await _context.SaveChangesAsync();
        }

        // Get order by id
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(a => a.Id == id);
            if (order is null)
                throw new Exception("Order doesn't Exist!");
            return order;
        }

        // Get user order details by orderId, including related entities
        public async Task<Order> GetOrderDetailsByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Where(a => a.Id == orderId)
                .Include(a => a.OrderStatus)
                .Include(a => a.OrderDetail)
                    .ThenInclude(od => od.Book)
                        .ThenInclude(b => b.Genre)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new Exception($"Order with id: {orderId} not found!");

            return order;
        }

        public async Task<IEnumerable<OrderStatus>> GetOrderStatuses()
        {
            return await _context.OrderStatuses.ToListAsync();
        }
    }
}
