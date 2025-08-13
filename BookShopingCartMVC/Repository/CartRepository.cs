using System.Net;
using System.Security.Cryptography.Xml;
using BookShopingCartMVC.Models.DTOs;
using BookShopingCartMVC.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace BookShopingCartMVC.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartRepository(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // add item to the Shopping Cart
        public async Task<int> AddItemAsync(int bookId, int quantity)
        {
            string userId = GetUserId();
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("Invalid User");

                var shoppingCart = await GetCartAsync();
                if (shoppingCart is null)
                {
                    shoppingCart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _context.ShoppingCarts.Add(shoppingCart);
                    await _context.SaveChangesAsync(); // Ensure the shoppingCart.Id is generated  
                }

                var cartItem = await _context.CartDetails.FirstOrDefaultAsync(a => a.ShoppingCartId == shoppingCart.Id && a.BookId == bookId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    var book = _context.Books.Find(bookId);
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = shoppingCart.Id,
                        Quantity = quantity,
                        UnitPrice = book!.Id == 0 ? 0 : book!.Price
                    };
                    _context.CartDetails.Add(cartItem);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error adding item to cart", ex);
            }
            return await GetCartItemCountAsync();
        }

        // Remove item from the Shopping Cart
        public async Task<int> RemoveItemAsync(int bookId)
        {
            try
            {
                string userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("Invalid User");

                var shoppingCart = await GetCartAsync();
                if (shoppingCart is null)
                    throw new Exception("Items in cart");

                _context.SaveChanges();
                var cartItem = await _context.CartDetails.FirstOrDefaultAsync(a => a.ShoppingCartId == shoppingCart.Id && a.BookId == bookId);

                if (cartItem is null)
                    return 0;
                else if (cartItem.Quantity == 1)
                    _context.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity = cartItem.Quantity - 1;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing item from cart", ex);
            }
            return await GetCartItemCountAsync();
        }
        // Remove item from the Shopping Cart
        public async Task<int> RemoveAllItemAsync()
        {
            try
            {
                var shoppingCart = await GetCartAsync();
                if (shoppingCart is null)
                    throw new Exception("Items in cart");

                _context.SaveChanges();
                var cartItems = await _context.CartDetails.Where(a => a.ShoppingCartId == shoppingCart.Id).ToListAsync();

                if (cartItems.Any())
                {
                    _context.CartDetails.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing item from cart", ex);
            }
        }

        // Get all Shopping Cart
        public async Task<ShoppingCart> GetAllCartAsync()
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("Invalid User");
            ShoppingCart? shoppingCart = await _context!.ShoppingCarts
                                  .Include(a => a.CartDetails!)
                                  .ThenInclude(a => a.Book)
                                  .ThenInclude(a => a!.Stock)
                                  .Include(a => a.CartDetails!)
                                  .ThenInclude(a => a.Book)
                                  .ThenInclude(a => a!.Genre)
                                  .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return shoppingCart!;
        }

        // Get all Cart Count
        public async Task<bool> DoCheckoutAsync(CheckoutModel model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var cart = await GetCartAsync();
                if (cart is null)
                    throw new Exception("Invalid Cart");

                var cartDetail = await _context.CartDetails
                    .Where(a => a.ShoppingCartId == cart.Id)
                    .ToListAsync();
                if (cartDetail.Count() == 0)
                    throw new Exception("Cart is Empty");

                var pendingRecord = _context.OrderStatuses.FirstOrDefault(s => s.StatusName == "Pending");
                if (pendingRecord is null)
                    throw new InvalidOperationException("Order status does not have Pending status");

                var order = new Order
                {
                    UserId = GetUserId(),
                    CreateDate = DateTime.UtcNow,
                    Name = model.Name,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    PaymentMethod = model.PaymentMethod,
                    Address = model.Address,
                    IsPaid = false,
                    OrderStatusId = pendingRecord.Id
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                foreach (var item in cartDetail)
                {
                    var orderDetails = new OrderDetail
                    {
                        BookId = item.BookId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                    };
                    _context.OrderDetails.Add(orderDetails);
                }
                _context.RemoveRange(cartDetail);

                await _context.SaveChangesAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner: " + ex.InnerException.Message);
                return false;
            }
        }

        // Get all Cart Count
        public async Task<int> GetCartItemCountAsync()
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("Invalid User");
            var shoppingCart = await (from cart in _context.ShoppingCarts
                                      join cartDetail in _context.CartDetails
                                      on cart.Id equals cartDetail.ShoppingCartId
                                      where cart.UserId == userId
                                      select cartDetail.Id).ToListAsync();

            return shoppingCart.Count();
        }

        // Get the Shopping Cart of the Authenticated User
        public async Task<ShoppingCart> GetCartAsync()
        {
            string userId = GetUserId();
            ShoppingCart? shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return shoppingCart!;
        }

        // Get the Authenticated User Id
        public string GetUserId()
        {
            var pranciple = _httpContextAccessor.HttpContext?.User;
            return _userManager.GetUserId(pranciple!)!;
        }
    }
}
