namespace Restaurant.REST.Models
{
    /// <summary>
    /// DTO для повної інформації про салат
    /// </summary>
    public class SaladDto : DishDto
    {
        public bool IsVegetarian { get; set; }
        public int Calories { get; set; }
        public string Dressing { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;

        public SaladDto()
        {
            DishType = "Salad";
        }
    }

    /// <summary>
    /// DTO для створення нового салату
    /// </summary>
    public class CreateSaladDto : CreateDishDto
    {
        public bool IsVegetarian { get; set; }
        public int Calories { get; set; }
        public string Dressing { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO для оновлення салату
    /// </summary>
    public class UpdateSaladDto : UpdateDishDto
    {
        public bool? IsVegetarian { get; set; }
        public int? Calories { get; set; }
        public string? Dressing { get; set; }
        public string? Ingredients { get; set; }
    }
}

