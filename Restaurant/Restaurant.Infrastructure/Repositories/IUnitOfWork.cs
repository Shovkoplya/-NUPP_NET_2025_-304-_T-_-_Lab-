namespace Restaurant.Infrastructure.Repositories
{
    /// <summary>
    /// Unit of Work патерн для координації роботи з репозиторіями
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Репозиторій клієнтів
        /// </summary>
        ICustomerRepository Customers { get; }

        /// <summary>
        /// Репозиторій страв
        /// </summary>
        IDishRepository Dishes { get; }

        /// <summary>
        /// Репозиторій замовлень
        /// </summary>
        IOrderRepository Orders { get; }

        /// <summary>
        /// Зберегти всі зміни в базі даних
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Почати транзакцію
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Підтвердити транзакцію
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Відкотити транзакцію
        /// </summary>
        Task RollbackTransactionAsync();
    }
}

