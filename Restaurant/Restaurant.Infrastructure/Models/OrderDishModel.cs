using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Infrastructure.Models
{
    // Проміжна таблиця для зв'язку багато-до-багатьох між OrderModel та DishModel
    [Table("OrderDishes")]
    public class OrderDishModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        // Зв'язок з OrderModel
        [Required]
        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        
        [Required]
        public OrderModel Order { get; set; } = null!;

        // Зв'язок з DishModel
        [Required]
        [ForeignKey("Dish")]
        public Guid DishId { get; set; }
        
        [Required]
        public DishModel Dish { get; set; } = null!;

        // Додаткові властивості для проміжної таблиці
        [Required]
        [Range(1, 100, ErrorMessage = "Кількість має бути від 1 до 100")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна має бути більше нуля")]
        [Column(TypeName = "decimal(18,2)")]
        public double PriceAtOrder { get; set; } // Ціна на момент замовлення

        [MaxLength(500)]
        public string? SpecialInstructions { get; set; }
    }
}

