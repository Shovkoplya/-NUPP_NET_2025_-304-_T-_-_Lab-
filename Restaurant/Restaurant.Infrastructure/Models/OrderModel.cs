using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Infrastructure.Models
{
    [Table("Orders")]
    public class OrderModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Загальна ціна не може бути від'ємною")]
        [Column(TypeName = "decimal(18,2)")]
        public double TotalPrice { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled

        [MaxLength(200)]
        public string? DeliveryAddress { get; set; }

        // Зв'язок багато-до-один з CustomerModel
        [Required]
        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        
        [Required]
        public CustomerModel Customer { get; set; } = null!;

        // Зв'язок багато-до-багатьох з DishModel через OrderDishModel
        public ICollection<OrderDishModel> OrderDishes { get; set; } = new List<OrderDishModel>();
    }
}

