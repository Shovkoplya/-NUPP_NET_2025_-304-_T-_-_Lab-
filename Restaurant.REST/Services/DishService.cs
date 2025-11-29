using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repositories;

namespace Restaurant.REST.Services
{
    /// <summary>
    /// Сервіс для роботи зі стравами
    /// </summary>
    public class DishService : IDishService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DishService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<bool> CreateAsync(DishModel element)
        {
            if (element == null)
                return false;

            try
            {
                await _unitOfWork.Dishes.AddAsync(element);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DishModel?> ReadAsync(Guid id)
        {
            return await _unitOfWork.Dishes.GetByIdAsync(id);
        }

        public async Task<IEnumerable<DishModel>> ReadAllAsync()
        {
            return await _unitOfWork.Dishes.GetAllAsync();
        }

        public async Task<IEnumerable<DishModel>> ReadAllAsync(int page, int amount)
        {
            if (page <= 0 || amount <= 0)
                return Enumerable.Empty<DishModel>();

            var allDishes = await _unitOfWork.Dishes.GetAllAsync();
            return allDishes.Skip((page - 1) * amount).Take(amount);
        }

        public async Task<bool> UpdateAsync(DishModel element)
        {
            if (element == null)
                return false;

            try
            {
                await _unitOfWork.Dishes.UpdateAsync(element);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(DishModel element)
        {
            if (element == null)
                return false;

            try
            {
                await _unitOfWork.Dishes.DeleteAsync(element);
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

        public async Task<IEnumerable<DishModel>> GetAvailableDishesAsync()
        {
            return await _unitOfWork.Dishes.GetAvailableDishesAsync();
        }

        public async Task<IEnumerable<PizzaModel>> GetAllPizzasAsync()
        {
            return await _unitOfWork.Dishes.GetAllPizzasAsync();
        }

        public async Task<IEnumerable<SaladModel>> GetAllSaladsAsync()
        {
            return await _unitOfWork.Dishes.GetAllSaladsAsync();
        }

        public async Task<IEnumerable<SaladModel>> GetVegetarianSaladsAsync()
        {
            return await _unitOfWork.Dishes.GetVegetarianSaladsAsync();
        }

        public async Task<bool> CreatePizzaAsync(PizzaModel pizza)
        {
            if (pizza == null)
                return false;

            try
            {
                await _unitOfWork.Dishes.AddAsync(pizza);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateSaladAsync(SaladModel salad)
        {
            if (salad == null)
                return false;

            try
            {
                await _unitOfWork.Dishes.AddAsync(salad);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}

