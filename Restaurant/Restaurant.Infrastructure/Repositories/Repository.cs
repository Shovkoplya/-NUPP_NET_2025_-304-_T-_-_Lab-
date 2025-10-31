using Microsoft.EntityFrameworkCore;

namespace Restaurant.Infrastructure.Repositories
{
    /// <summary>
    /// Базова реалізація репозиторію з використанням RestaurantContext
    /// </summary>
    /// <typeparam name="T">Тип сутності</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly RestaurantContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(RestaurantContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Отримати сутність за ідентифікатором
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Отримати всі сутності
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Додати нову сутність
        /// </summary>
        public virtual async Task AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Оновити існуючу сутність
        /// </summary>
        public virtual Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Видалити сутність
        /// </summary>
        public virtual Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Видалити сутність за ідентифікатором
        /// </summary>
        public virtual async Task DeleteByIdAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        /// <summary>
        /// Зберегти зміни в базі даних
        /// </summary>
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

