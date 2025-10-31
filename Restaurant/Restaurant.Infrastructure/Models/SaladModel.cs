using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Infrastructure.Models
{
    // Table-per-Type (TPT) - окрема таблиця для Salad
    [Table("Salads")]
    public class SaladModel : DishModel
    {
        public bool IsVegetarian { get; set; }

        [Required]
        [Range(0, 2000, ErrorMessage = "Калорії мають бути від 0 до 2000")]
        public int Calories { get; set; }

        [Required]
        [MaxLength(100)]
        public string Dressing { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Ingredients { get; set; } = string.Empty;
    }
}

