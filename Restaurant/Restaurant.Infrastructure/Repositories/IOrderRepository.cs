using Restaurant.Infrastructure.Models;

namespace Restaurant.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторій для роботи із замовленнями
    /// </summary>
    public interface IOrderRepository : IRepository<OrderModel>
    {
        /// <summary>
        /// Отримати замовлення з деталями (страви)
        /// </summary>
        Task<OrderModel?> GetWithDetailsAsync(Guid id);

        /// <summary>
        /// Отримати замовлення клієнта
        /// </summary>
        Task<IEnumerable<OrderModel>> GetCustomerOrdersAsync(Guid customerId);

        /// <summary>
        /// Отримати замовлення за статусом
        /// </summary>
        Task<IEnumerable<OrderModel>> GetOrdersByStatusAsync(string status);

        /// <summary>
        /// Отримати замовлення за період
        /// </summary>
        Task<IEnumerable<OrderModel>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Отримати загальну суму замовлень клієнта
        /// </summary>
        Task<double> GetTotalSpentByCustomerAsync(Guid customerId);

        /// <summary>
        /// Отримати останні замовлення
        /// </summary>
        Task<IEnumerable<OrderModel>> GetRecentOrdersAsync(int count);
    }
}

