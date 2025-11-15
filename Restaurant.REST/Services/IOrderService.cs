using Restaurant.Infrastructure.Models;

namespace Restaurant.REST.Services
{
    /// <summary>
    /// Інтерфейс сервісу для роботи із замовленнями
    /// </summary>
    public interface IOrderService : ICrudServiceAsync<OrderModel>
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
        /// Додати страву до замовлення
        /// </summary>
        Task<bool> AddDishToOrderAsync(Guid orderId, Guid dishId, int quantity, string? specialInstructions = null);

        /// <summary>
        /// Видалити страву з замовлення
        /// </summary>
        Task<bool> RemoveDishFromOrderAsync(Guid orderId, Guid orderDishId);

        /// <summary>
        /// Оновити статус замовлення
        /// </summary>
        Task<bool> UpdateOrderStatusAsync(Guid orderId, string status);
    }
}

