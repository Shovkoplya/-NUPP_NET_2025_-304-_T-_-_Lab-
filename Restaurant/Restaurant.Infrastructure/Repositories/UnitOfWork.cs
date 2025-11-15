using Microsoft.EntityFrameworkCore.Storage;

namespace Restaurant.Infrastructure.Repositories
{
    /// <summary>
    /// Реалізація Unit of Work патерну для координації роботи з репозиторіями
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantContext _context;
        private IDbContextTransaction? _transaction;

        private ICustomerRepository? _customerRepository;
        private IDishRepository? _dishRepository;
        private IOrderRepository? _orderRepository;

        public UnitOfWork(RestaurantContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Репозиторій клієнтів
        /// </summary>
        public ICustomerRepository Customers
        {
            get
            {
                _customerRepository ??= new CustomerRepository(_context);
                return _customerRepository;
            }
        }

        /// <summary>
        /// Репозиторій страв
        /// </summary>
        public IDishRepository Dishes
        {
            get
            {
                _dishRepository ??= new DishRepository(_context);
                return _dishRepository;
            }
        }

        /// <summary>
        /// Репозиторій замовлень
        /// </summary>
        public IOrderRepository Orders
        {
            get
            {
                _orderRepository ??= new OrderRepository(_context);
                return _orderRepository;
            }
        }

        /// <summary>
        /// Зберегти всі зміни в базі даних
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Почати транзакцію
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Підтвердити транзакцію
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Відкотити транзакцію
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Звільнити ресурси
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

