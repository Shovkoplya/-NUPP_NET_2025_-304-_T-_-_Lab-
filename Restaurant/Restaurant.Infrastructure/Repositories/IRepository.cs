namespace Restaurant.Infrastructure.Repositories
{
    /// <summary>
    /// Базовий інтерфейс репозиторію для роботи з сутностями
    /// </summary>
    /// <typeparam name="T">Тип сутності</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Отримати сутність за ідентифікатором
        /// </summary>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Отримати всі сутності
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Додати нову сутність
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Оновити існуючу сутність
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Видалити сутність
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Видалити сутність за ідентифікатором
        /// </summary>
        Task DeleteByIdAsync(Guid id);

        /// <summary>
        /// Зберегти зміни в базі даних
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}

