namespace Restaurant.REST.Models
{
    /// <summary>
    /// DTO для повної інформації про страву
    /// </summary>
    public class DishDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public string DishType { get; set; } = "Dish"; // Dish, Pizza, Salad
    }

    /// <summary>
    /// DTO для створення нової страви
    /// </summary>
    public class CreateDishDto
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
    }

    /// <summary>
    /// DTO для оновлення страви
    /// </summary>
    public class UpdateDishDto
    {
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? Description { get; set; }
        public bool? IsAvailable { get; set; }
    }
}

