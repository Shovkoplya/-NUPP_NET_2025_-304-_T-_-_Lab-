using Restaurant.Infrastructure.Models;

namespace Restaurant.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторій для роботи з клієнтами
    /// </summary>
    public interface ICustomerRepository : IRepository<CustomerModel>
    {
        /// <summary>
        /// Отримати клієнта за номером телефону
        /// </summary>
        Task<CustomerModel?> GetByPhoneNumberAsync(string phoneNumber);

        /// <summary>
        /// Отримати клієнта з профілем
        /// </summary>
        Task<CustomerModel?> GetWithProfileAsync(Guid id);

        /// <summary>
        /// Отримати клієнта з усіма замовленнями
        /// </summary>
        Task<CustomerModel?> GetWithOrdersAsync(Guid id);

        /// <summary>
        /// Отримати топ клієнтів за балами лояльності
        /// </summary>
        Task<IEnumerable<CustomerModel>> GetTopLoyalCustomersAsync(int count);
    }
}

