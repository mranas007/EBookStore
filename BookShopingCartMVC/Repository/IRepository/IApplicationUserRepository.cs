using BookShopingCartMVC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookShopingCartMVC.Repository.IRepository
{
    public interface IApplicationUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
    }
}