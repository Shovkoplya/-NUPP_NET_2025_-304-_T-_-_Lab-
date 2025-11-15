using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Infrastructure.Models
{
    [Table("Customers")]
    public class CustomerModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Повне ім'я обов'язкове")]
        [MaxLength(100, ErrorMessage = "Ім'я не може перевищувати 100 символів")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Номер телефону обов'язковий")]
        [Phone(ErrorMessage = "Невірний формат номера телефону")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Бали лояльності не можуть бути від'ємними")]
        public int LoyaltyPoints { get; set; }

        // Зв'язок один-до-одного з CustomerProfileModel
        public CustomerProfileModel? Profile { get; set; }

        // Зв'язок один-до-багатьох з OrderModel
        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
    }
}

