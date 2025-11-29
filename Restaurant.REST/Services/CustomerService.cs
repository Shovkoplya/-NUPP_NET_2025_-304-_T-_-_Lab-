using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repositories;

namespace Restaurant.REST.Services
{
    /// <summary>
    /// Сервіс для роботи з клієнтами
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<bool> CreateAsync(CustomerModel element)
        {
            if (element == null)
                return false;

            try
            {
                await _unitOfWork.Customers.AddAsync(element);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<CustomerModel?> ReadAsync(Guid id)
        {
            return await _unitOfWork.Customers.GetByIdAsync(id);
        }

        public async Task<IEnumerable<CustomerModel>> ReadAllAsync()
        {
            return await _unitOfWork.Customers.GetAllAsync();
        }

        public async Task<IEnumerable<CustomerModel>> ReadAllAsync(int page, int amount)
        {
            if (page <= 0 || amount <= 0)
                return Enumerable.Empty<CustomerModel>();

            var allCustomers = await _unitOfWork.Customers.GetAllAsync();
            return allCustomers.Skip((page - 1) * amount).Take(amount);
        }

        public async Task<bool> UpdateAsync(CustomerModel element)
        {
            if (element == null)
                return false;

            try
            {
                await _unitOfWork.Customers.UpdateAsync(element);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(CustomerModel element)
        {
            if (element == null)
                return false;

            try
            {
                await _unitOfWork.Customers.DeleteAsync(element);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                var result = await _unitOfWork.SaveChangesAsync();
                return result >= 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<CustomerModel?> GetWithProfileAsync(Guid id)
        {
            return await _unitOfWork.Customers.GetWithProfileAsync(id);
        }

        public async Task<CustomerModel?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _unitOfWork.Customers.GetByPhoneNumberAsync(phoneNumber);
        }

        public async Task<bool> CreateProfileAsync(Guid customerId, CustomerProfileModel profile)
        {
            if (profile == null)
                return false;

            try
            {
                profile.CustomerId = customerId;
                
                // Використовуємо контекст через рефлексію або додамо метод у репозиторій
                // Для простоти, використаємо прямий доступ через UnitOfWork
                // Потрібно додати метод в CustomerRepository або використати загальний підхід
                
                // Тимчасове рішення - використаємо базовий репозиторій через UnitOfWork
                // В ідеалі потрібно розширити ICustomerRepository
                return false; // Буде реалізовано пізніше
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProfileAsync(CustomerProfileModel profile)
        {
            if (profile == null)
                return false;

            try
            {
                // Аналогічно до CreateProfileAsync
                return false; // Буде реалізовано пізніше
            }
            catch
            {
                return false;
            }
        }
    }
}

