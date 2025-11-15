using Restaurant.Infrastructure.Models;

namespace Restaurant.REST.Services
{
    /// <summary>
    /// Інтерфейс сервісу для роботи з клієнтами
    /// </summary>
    public interface ICustomerService : ICrudServiceAsync<CustomerModel>
    {
        /// <summary>
        /// Отримати клієнта з профілем
        /// </summary>
        Task<CustomerModel?> GetWithProfileAsync(Guid id);

        /// <summary>
        /// Отримати клієнта за номером телефону
        /// </summary>
        Task<CustomerModel?> GetByPhoneNumberAsync(string phoneNumber);

        /// <summary>
        /// Створити профіль для клієнта
        /// </summary>
        Task<bool> CreateProfileAsync(Guid customerId, CustomerProfileModel profile);

        /// <summary>
        /// Оновити профіль клієнта
        /// </summary>
        Task<bool> UpdateProfileAsync(CustomerProfileModel profile);
    }
}

