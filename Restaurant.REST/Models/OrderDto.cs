namespace Restaurant.REST.Models
{
    /// <summary>
    /// DTO для повної інформації про замовлення
    /// </summary>
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? DeliveryAddress { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

    /// <summary>
    /// DTO для позиції замовлення (страва в замовленні)
    /// </summary>
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid DishId { get; set; }
        public string DishName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double PriceAtOrder { get; set; }
        public string? SpecialInstructions { get; set; }
    }

    /// <summary>
    /// DTO для створення нового замовлення
    /// </summary>
    public class CreateOrderDto
    {
        public Guid CustomerId { get; set; }
        public string? DeliveryAddress { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }

    /// <summary>
    /// DTO для створення позиції замовлення
    /// </summary>
    public class CreateOrderItemDto
    {
        public Guid DishId { get; set; }
        public int Quantity { get; set; } = 1;
        public string? SpecialInstructions { get; set; }
    }

    /// <summary>
    /// DTO для оновлення замовлення
    /// </summary>
    public class UpdateOrderDto
    {
        public string? Status { get; set; }
        public string? DeliveryAddress { get; set; }
    }

    /// <summary>
    /// DTO для додавання позиції до існуючого замовлення
    /// </summary>
    public class AddOrderItemDto
    {
        public Guid DishId { get; set; }
        public int Quantity { get; set; } = 1;
        public string? SpecialInstructions { get; set; }
    }
}

