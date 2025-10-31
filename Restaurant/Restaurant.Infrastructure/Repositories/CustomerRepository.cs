using Microsoft.EntityFrameworkCore;
using Restaurant.Infrastructure.Models;

namespace Restaurant.Infrastructure.Repositories
{
    /// <summary>
    /// Реалізація репозиторію для роботи з клієнтами
    /// </summary>
    public class CustomerRepository : Repository<CustomerModel>, ICustomerRepository
    {
        public CustomerRepository(RestaurantContext context) : base(context)
        {
        }

        /// <summary>
        /// Отримати клієнта за номером телефону
        /// </summary>
        public async Task<CustomerModel?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
        }

        /// <summary>
        /// Отримати клієнта з профілем
        /// </summary>
        public async Task<CustomerModel?> GetWithProfileAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Отримати клієнта з усіма замовленнями
        /// </summary>
        public async Task<CustomerModel?> GetWithOrdersAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderDishes)
                        .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Отримати топ клієнтів за балами лояльності
        /// </summary>
        public async Task<IEnumerable<CustomerModel>> GetTopLoyalCustomersAsync(int count)
        {
            return await _dbSet
                .OrderByDescending(c => c.LoyaltyPoints)
                .Take(count)
                .ToListAsync();
        }
    }
}

