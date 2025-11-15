namespace Restaurant.REST.Services
{
    /// <summary>
    /// Інтерфейс асинхронного CRUD сервісу
    /// </summary>
    /// <typeparam name="T">Тип сутності</typeparam>
    public interface ICrudServiceAsync<T> where T : class
    {
        /// <summary>
        /// Створити нову сутність
        /// </summary>
        Task<bool> CreateAsync(T element);

        /// <summary>
        /// Прочитати сутність за ID
        /// </summary>
        Task<T?> ReadAsync(Guid id);

        /// <summary>
        /// Прочитати всі сутності
        /// </summary>
        Task<IEnumerable<T>> ReadAllAsync();

        /// <summary>
        /// Прочитати сутності з пагінацією
        /// </summary>
        Task<IEnumerable<T>> ReadAllAsync(int page, int amount);

        /// <summary>
        /// Оновити сутність
        /// </summary>
        Task<bool> UpdateAsync(T element);

        /// <summary>
        /// Видалити сутність
        /// </summary>
        Task<bool> RemoveAsync(T element);

        /// <summary>
        /// Зберегти зміни
        /// </summary>
        Task<bool> SaveAsync();
    }
}

