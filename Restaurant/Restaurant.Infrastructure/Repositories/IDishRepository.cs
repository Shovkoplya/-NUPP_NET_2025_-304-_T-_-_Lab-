using Restaurant.Infrastructure.Models;

namespace Restaurant.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторій для роботи зі стравами
    /// </summary>
    public interface IDishRepository : IRepository<DishModel>
    {
        /// <summary>
        /// Отримати всі доступні страви
        /// </summary>
        Task<IEnumerable<DishModel>> GetAvailableDishesAsync();

        /// <summary>
        /// Отримати страви за ціновим діапазоном
        /// </summary>
        Task<IEnumerable<DishModel>> GetDishesByPriceRangeAsync(double minPrice, double maxPrice);

        /// <summary>
        /// Отримати страву за назвою
        /// </summary>
        Task<DishModel?> GetByNameAsync(string name);

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
    }
}

