using Microsoft.EntityFrameworkCore;
using Restaurant.Infrastructure.Models;

namespace Restaurant.Infrastructure.Repositories
{
    /// <summary>
    /// Реалізація репозиторію для роботи зі стравами
    /// </summary>
    public class DishRepository : Repository<DishModel>, IDishRepository
    {
        public DishRepository(RestaurantContext context) : base(context)
        {
        }

        /// <summary>
        /// Отримати всі доступні страви
        /// </summary>
        public async Task<IEnumerable<DishModel>> GetAvailableDishesAsync()
        {
            return await _dbSet
                .Where(d => d.IsAvailable)
                .ToListAsync();
        }

        /// <summary>
        /// Отримати страви за ціновим діапазоном
        /// </summary>
        public async Task<IEnumerable<DishModel>> GetDishesByPriceRangeAsync(double minPrice, double maxPrice)
        {
            return await _dbSet
                .Where(d => d.Price >= minPrice && d.Price <= maxPrice)
                .OrderBy(d => d.Price)
                .ToListAsync();
        }

        /// <summary>
        /// Отримати страву за назвою
        /// </summary>
        public async Task<DishModel?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.Name == name);
        }

        /// <summary>
        /// Отримати всі піци
        /// </summary>
        public async Task<IEnumerable<PizzaModel>> GetAllPizzasAsync()
        {
            return await _context.Pizzas
                .Where(p => p.IsAvailable)
                .ToListAsync();
        }

        /// <summary>
        /// Отримати всі салати
        /// </summary>
        public async Task<IEnumerable<SaladModel>> GetAllSaladsAsync()
        {
            return await _context.Salads
                .Where(s => s.IsAvailable)
                .ToListAsync();
        }

        /// <summary>
        /// Отримати вегетаріанські салати
        /// </summary>
        public async Task<IEnumerable<SaladModel>> GetVegetarianSaladsAsync()
        {
            return await _context.Salads
                .Where(s => s.IsVegetarian && s.IsAvailable)
                .ToListAsync();
        }
    }
}

