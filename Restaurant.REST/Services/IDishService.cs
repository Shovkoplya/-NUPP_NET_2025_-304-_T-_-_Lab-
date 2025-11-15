using Restaurant.Infrastructure.Models;

namespace Restaurant.REST.Services
{
    /// <summary>
    /// Інтерфейс сервісу для роботи зі стравами
    /// </summary>
    public interface IDishService : ICrudServiceAsync<DishModel>
    {
        /// <summary>
        /// Отримати всі доступні страви
        /// </summary>
        Task<IEnumerable<DishModel>> GetAvailableDishesAsync();

        /// <summary>
        /// Отримати всі піци
        /// </summary>
        Task<IEnumerable<PizzaModel>> GetAllPizzasAsync();

        /// <summary>
        /// Отримати всі салати
        /// </summary>
        Task<IEnumerable<SaladModel>> GetAllSaladsAsync();

        /// <summary>
        /// Отримати вегетаріанські салати
        /// </summary>
        Task<IEnumerable<SaladModel>> GetVegetarianSaladsAsync();

        /// <summary>
        /// Створити піцу
        /// </summary>
        Task<bool> CreatePizzaAsync(PizzaModel pizza);

        /// <summary>
        /// Створити салат
        /// </summary>
        Task<bool> CreateSaladAsync(SaladModel salad);
    }
}

