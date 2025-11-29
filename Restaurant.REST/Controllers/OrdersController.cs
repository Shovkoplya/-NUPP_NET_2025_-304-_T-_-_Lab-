using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.REST.Models;
using Restaurant.REST.Services;
using Restaurant.Infrastructure.Models;

namespace Restaurant.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IDishService _dishService;

        public OrdersController(
            IOrderService orderService,
            ICustomerService customerService,
            IDishService dishService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _dishService = dishService ?? throw new ArgumentNullException(nameof(dishService));
        }

        /// <summary>
        /// Отримати всі замовлення (тільки для адміністраторів та менеджерів)
        /// </summary>
        /// <param name="customerId">Фільтр за ID клієнта</param>
        /// <param name="status">Фільтр за статусом</param>
        /// <returns>Список замовлень</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(
            [FromQuery] Guid? customerId = null,
            [FromQuery] string? status = null)
        {
            IEnumerable<OrderModel> orders;

            if (customerId.HasValue)
            {
                orders = await _orderService.GetCustomerOrdersAsync(customerId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(status))
            {
                orders = await _orderService.GetOrdersByStatusAsync(status);
            }
            else
            {
                orders = await _orderService.ReadAllAsync();
            }

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders.OrderByDescending(o => o.OrderDate))
            {
                orderDtos.Add(await MapToDtoAsync(order));
            }

            return Ok(orderDtos);
        }

        /// <summary>
        /// Отримати замовлення за ID (всі авторизовані користувачі)
        /// </summary>
        /// <param name="id">ID замовлення</param>
        /// <returns>Дані замовлення</returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderDto>> GetById(Guid id)
        {
            var order = await _orderService.GetWithDetailsAsync(id);
            if (order == null)
            {
                return NotFound(new { message = $"Замовлення з ID {id} не знайдено" });
            }

            return Ok(await MapToDtoAsync(order));
        }

        /// <summary>
        /// Створити нове замовлення (всі авторизовані користувачі)
        /// </summary>
        /// <param name="createDto">Дані для створення замовлення</param>
        /// <returns>Створене замовлення</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto createDto)
        {
            // Перевірка клієнта
            var customer = await _customerService.ReadAsync(createDto.CustomerId);
            if (customer == null)
            {
                return BadRequest(new { message = $"Клієнта з ID {createDto.CustomerId} не знайдено" });
            }

            if (createDto.Items == null || createDto.Items.Count == 0)
            {
                return BadRequest(new { message = "Замовлення має містити хоча б одну страву" });
            }

            // Створюємо замовлення
            var order = new OrderModel
            {
                Id = Guid.NewGuid(),
                CustomerId = createDto.CustomerId,
                Customer = customer,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                DeliveryAddress = createDto.DeliveryAddress,
                TotalPrice = 0,
                OrderDishes = new List<OrderDishModel>()
            };

            // Додаємо позиції замовлення
            foreach (var item in createDto.Items)
            {
                var dish = await _dishService.ReadAsync(item.DishId);
                if (dish == null)
                {
                    return BadRequest(new { message = $"Страву з ID {item.DishId} не знайдено" });
                }

                if (!dish.IsAvailable)
                {
                    return BadRequest(new { message = $"Страва '{dish.Name}' недоступна" });
                }

                if (item.Quantity <= 0)
                {
                    return BadRequest(new { message = "Кількість має бути більше нуля" });
                }

                var orderDish = new OrderDishModel
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    DishId = dish.Id,
                    Dish = dish,
                    Quantity = item.Quantity,
                    PriceAtOrder = dish.Price,
                    SpecialInstructions = item.SpecialInstructions
                };

                order.OrderDishes.Add(orderDish);
                order.TotalPrice += dish.Price * item.Quantity;
            }

            var created = await _orderService.CreateAsync(order);
            if (!created)
            {
                return BadRequest(new { message = "Не вдалося створити замовлення" });
            }

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, await MapToDtoAsync(order));
        }

        /// <summary>
        /// Оновити замовлення - статус, адресу (тільки для адміністраторів та менеджерів)
        /// </summary>
        /// <param name="id">ID замовлення</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <returns>Оновлене замовлення</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<OrderDto>> Update(Guid id, [FromBody] UpdateOrderDto updateDto)
        {
            var order = await _orderService.GetWithDetailsAsync(id);
            if (order == null)
            {
                return NotFound(new { message = $"Замовлення з ID {id} не знайдено" });
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Status))
            {
                var validStatuses = new[] { "Pending", "Preparing", "Ready", "Delivering", "Completed", "Cancelled" };
                if (!validStatuses.Contains(updateDto.Status, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = $"Невірний статус. Допустимі: {string.Join(", ", validStatuses)}" });
                }
                order.Status = updateDto.Status;
            }

            if (updateDto.DeliveryAddress != null)
            {
                order.DeliveryAddress = updateDto.DeliveryAddress;
            }

            var updated = await _orderService.UpdateAsync(order);
            if (!updated)
            {
                return BadRequest(new { message = "Не вдалося оновити замовлення" });
            }

            return Ok(await MapToDtoAsync(order));
        }

        /// <summary>
        /// Видалити замовлення (тільки для адміністраторів)
        /// </summary>
        /// <param name="id">ID замовлення</param>
        /// <returns>Статус видалення</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var order = await _orderService.ReadAsync(id);
            if (order == null)
            {
                return NotFound(new { message = $"Замовлення з ID {id} не знайдено" });
            }

            // Не дозволяємо видаляти замовлення, які вже в роботі
            if (order.Status != "Pending" && order.Status != "Cancelled")
            {
                return BadRequest(new { message = "Можна видалити тільки замовлення зі статусом 'Pending' або 'Cancelled'" });
            }

            var deleted = await _orderService.RemoveAsync(order);
            if (!deleted)
            {
                return BadRequest(new { message = "Не вдалося видалити замовлення" });
            }

            return NoContent();
        }

        /// <summary>
        /// Додати страву до замовлення (всі авторизовані користувачі)
        /// </summary>
        /// <param name="id">ID замовлення</param>
        /// <param name="addItemDto">Дані страви для додавання</param>
        /// <returns>Оновлене замовлення</returns>
        [HttpPost("{id}/items")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderDto>> AddItem(Guid id, [FromBody] AddOrderItemDto addItemDto)
        {
            var order = await _orderService.GetWithDetailsAsync(id);
            if (order == null)
            {
                return NotFound(new { message = $"Замовлення з ID {id} не знайдено" });
            }

            if (order.Status != "Pending")
            {
                return BadRequest(new { message = "Можна додавати страви тільки до замовлень зі статусом 'Pending'" });
            }

            var added = await _orderService.AddDishToOrderAsync(id, addItemDto.DishId, addItemDto.Quantity, addItemDto.SpecialInstructions);
            if (!added)
            {
                return BadRequest(new { message = "Не вдалося додати страву до замовлення" });
            }

            var updatedOrder = await _orderService.GetWithDetailsAsync(id);
            return Ok(await MapToDtoAsync(updatedOrder!));
        }

        /// <summary>
        /// Видалити страву з замовлення (всі авторизовані користувачі)
        /// </summary>
        /// <param name="id">ID замовлення</param>
        /// <param name="itemId">ID позиції замовлення</param>
        /// <returns>Оновлене замовлення</returns>
        [HttpDelete("{id}/items/{itemId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderDto>> RemoveItem(Guid id, Guid itemId)
        {
            var order = await _orderService.GetWithDetailsAsync(id);
            if (order == null)
            {
                return NotFound(new { message = $"Замовлення з ID {id} не знайдено" });
            }

            if (order.Status != "Pending")
            {
                return BadRequest(new { message = "Можна видаляти страви тільки з замовлень зі статусом 'Pending'" });
            }

            if (order.OrderDishes.Count <= 1)
            {
                return BadRequest(new { message = "Не можна видалити останню страву. Видаліть замовлення повністю." });
            }

            var removed = await _orderService.RemoveDishFromOrderAsync(id, itemId);
            if (!removed)
            {
                return BadRequest(new { message = "Не вдалося видалити страву з замовлення" });
            }

            var updatedOrder = await _orderService.GetWithDetailsAsync(id);
            return Ok(await MapToDtoAsync(updatedOrder!));
        }

        /// <summary>
        /// Отримати статистику замовлень (тільки для адміністраторів та менеджерів)
        /// </summary>
        /// <returns>Статистика</returns>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<object>> GetStatistics()
        {
            var allOrders = await _orderService.ReadAllAsync();
            var ordersList = allOrders.ToList();

            var stats = new
            {
                TotalOrders = ordersList.Count,
                PendingOrders = ordersList.Count(o => o.Status == "Pending"),
                CompletedOrders = ordersList.Count(o => o.Status == "Completed"),
                CancelledOrders = ordersList.Count(o => o.Status == "Cancelled"),
                TotalRevenue = ordersList.Where(o => o.Status == "Completed").Sum(o => o.TotalPrice),
                AverageOrderValue = ordersList.Any() ? ordersList.Average(o => o.TotalPrice) : 0
            };

            return Ok(stats);
        }

        #region Helper Methods

        private async Task<OrderDto> MapToDtoAsync(OrderModel order)
        {
            var customer = order.Customer ?? await _customerService.ReadAsync(order.CustomerId);

            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                DeliveryAddress = order.DeliveryAddress,
                CustomerId = order.CustomerId,
                CustomerName = customer?.FullName ?? "Unknown",
                Items = order.OrderDishes.Select(od => new OrderItemDto
                {
                    Id = od.Id,
                    DishId = od.DishId,
                    DishName = od.Dish?.Name ?? "Unknown",
                    Quantity = od.Quantity,
                    PriceAtOrder = od.PriceAtOrder,
                    SpecialInstructions = od.SpecialInstructions
                }).ToList()
            };
        }

        #endregion
    }
}
