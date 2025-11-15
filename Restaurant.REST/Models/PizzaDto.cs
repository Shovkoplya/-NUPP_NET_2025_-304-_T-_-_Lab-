namespace Restaurant.REST.Models
{
    /// <summary>
    /// DTO для повної інформації про піцу
    /// </summary>
    public class PizzaDto : DishDto
    {
        public int SizeCm { get; set; }
        public string DoughType { get; set; } = string.Empty;
        public bool ExtraCheese { get; set; }
        public string Toppings { get; set; } = string.Empty;

        public PizzaDto()
        {
            DishType = "Pizza";
        }
    }

    /// <summary>
    /// DTO для створення нової піці
    /// </summary>
    public class CreatePizzaDto : CreateDishDto
    {
        public int SizeCm { get; set; }
        public string DoughType { get; set; } = string.Empty;
        public bool ExtraCheese { get; set; }
        public string Toppings { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO для оновлення піці
    /// </summary>
    public class UpdatePizzaDto : UpdateDishDto
    {
        public int? SizeCm { get; set; }
        public string? DoughType { get; set; }
        public bool? ExtraCheese { get; set; }
        public string? Toppings { get; set; }
    }
}

