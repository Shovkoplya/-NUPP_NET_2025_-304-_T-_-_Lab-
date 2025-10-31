using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Infrastructure.Models
{
    // Table-per-Type (TPT) - окрема таблиця для Pizza
    [Table("Pizzas")]
    public class PizzaModel : DishModel
    {
        [Required]
        [Range(20, 50, ErrorMessage = "Розмір піци має бути від 20 до 50 см")]
        public int SizeCm { get; set; }

        [Required]
        [MaxLength(50)]
        public string DoughType { get; set; } = string.Empty;

        public bool ExtraCheese { get; set; }

        [MaxLength(200)]
        public string Toppings { get; set; } = string.Empty;
    }
}

