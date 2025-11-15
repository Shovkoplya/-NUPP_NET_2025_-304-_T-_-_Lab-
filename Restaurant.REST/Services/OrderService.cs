using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repositories;

namespace Restaurant.REST.Services
{
    /// <summary>
    /// Сервіс для роботи із замовленнями
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<bool> CreateAsync(OrderModel element)
        {
            if (element == null)
                return false;

            try
            {
                await _unitOfWork.Orders.AddAsync(element);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<OrderModel?> ReadAsync(Guid id)
        {
            return await _unitOfWork.Orders.GetByIdAsync(id);
        }

        public async Task<IEnumerable<OrderModel>> ReadAllAsync()
        {
            return await _unitOfWork.Orders.GetAllAsync();
        }

        public async Task<IEnumerable<OrderModel>> ReadAllAsync(int page, int amount)
        {
            if (page <= 0 || amount <= 0)
                return Enumerable.Empty<OrderModel>();

            var allOrders = await _unitOfWork.Orders.GetAllAsync();
            return allOrders.Skip((page - 1) * amount).Take(amount);
        }

        public async Task<bool> UpdateAsync(OrderModel element)
        {
            if (element == null)
                return false;

            try
            {
                await _unitOfWork.Orders.UpdateAsync(element);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(OrderModel element)
        {
            if (element == null)
                return false;

            try
            {
                await _unitOfWork.Orders.DeleteAsync(element);
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

        public async Task<OrderModel?> GetWithDetailsAsync(Guid id)
        {
            return await _unitOfWork.Orders.GetWithDetailsAsync(id);
        }

        public async Task<IEnumerable<OrderModel>> GetCustomerOrdersAsync(Guid customerId)
        {
            return await _unitOfWork.Orders.GetCustomerOrdersAsync(customerId);
        }

        public async Task<IEnumerable<OrderModel>> GetOrdersByStatusAsync(string status)
        {
            return await _unitOfWork.Orders.GetOrdersByStatusAsync(status);
        }

        public async Task<bool> AddDishToOrderAsync(Guid orderId, Guid dishId, int quantity, string? specialInstructions = null)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetWithDetailsAsync(orderId);
                if (order == null)
                    return false;

                var dish = await _unitOfWork.Dishes.GetByIdAsync(dishId);
                if (dish == null || !dish.IsAvailable)
                    return false;

                var orderDish = new OrderDishModel
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    DishId = dishId,
                    Quantity = quantity,
                    PriceAtOrder = dish.Price,
                    SpecialInstructions = specialInstructions
                };

                order.OrderDishes.Add(orderDish);
                order.TotalPrice += dish.Price * quantity;

                await _unitOfWork.Orders.UpdateAsync(order);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveDishFromOrderAsync(Guid orderId, Guid orderDishId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetWithDetailsAsync(orderId);
                if (order == null)
                    return false;

                var orderDish = order.OrderDishes.FirstOrDefault(od => od.Id == orderDishId);
                if (orderDish == null)
                    return false;

                order.TotalPrice -= orderDish.PriceAtOrder * orderDish.Quantity;
                order.OrderDishes.Remove(orderDish);

                await _unitOfWork.Orders.UpdateAsync(order);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string status)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                    return false;

                order.Status = status;
                await _unitOfWork.Orders.UpdateAsync(order);
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

