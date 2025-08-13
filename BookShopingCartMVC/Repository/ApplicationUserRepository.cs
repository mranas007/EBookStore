using BookShopingCartMVC.Data;
using BookShopingCartMVC.Models;
using BookShopingCartMVC.Repository.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookShopingCartMVC.Repository
{
    public class ApplicationUserRepository :  IApplicationUserRepository
    {
        private ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _context.ApplicationUsers.ToListAsync();
        }
    }
}