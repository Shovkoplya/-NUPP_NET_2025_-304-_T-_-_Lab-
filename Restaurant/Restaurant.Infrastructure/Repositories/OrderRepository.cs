using Microsoft.EntityFrameworkCore;
using Restaurant.Infrastructure.Models;

namespace Restaurant.Infrastructure.Repositories
{
    /// <summary>
    /// Реалізація репозиторію для роботи із замовленнями
    /// </summary>
    public class OrderRepository : Repository<OrderModel>, IOrderRepository
    {
        public OrderRepository(RestaurantContext context) : base(context)
        {
        }

        /// <summary>
        /// Отримати замовлення з деталями (страви)
        /// </summary>
        public async Task<OrderModel?> GetWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(o => o.Customer)
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        /// <summary>
        /// Отримати замовлення клієнта
        /// </summary>
        public async Task<IEnumerable<OrderModel>> GetCustomerOrdersAsync(Guid customerId)
        {
            return await _dbSet
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        /// <summary>
        /// Отримати замовлення за статусом
        /// </summary>
        public async Task<IEnumerable<OrderModel>> GetOrdersByStatusAsync(string status)
        {
            return await _dbSet
                .Include(o => o.Customer)
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        /// <summary>
        /// Отримати замовлення за період
        /// </summary>
        public async Task<IEnumerable<OrderModel>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(o => o.Customer)
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        /// <summary>
        /// Отримати загальну суму замовлень клієнта
        /// </summary>
        public async Task<double> GetTotalSpentByCustomerAsync(Guid customerId)
        {
            return await _dbSet
                .Where(o => o.CustomerId == customerId && o.Status == "Completed")
                .SumAsync(o => o.TotalPrice);
        }

        /// <summary>
        /// Отримати останні замовлення
        /// </summary>
        public async Task<IEnumerable<OrderModel>> GetRecentOrdersAsync(int count)
        {
            return await _dbSet
                .Include(o => o.Customer)
                .Include(o => o.OrderDishes)
                    .ThenInclude(od => od.Dish)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }
    }
}

