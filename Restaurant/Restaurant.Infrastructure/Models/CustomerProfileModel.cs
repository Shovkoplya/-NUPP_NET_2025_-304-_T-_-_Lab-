using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Infrastructure.Models
{
    [Table("CustomerProfiles")]
    public class CustomerProfileModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PreferredPaymentMethod { get; set; } = string.Empty;

        // Зв'язок один-до-одного з CustomerModel (зворотній)
        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        
        [Required]
        public CustomerModel Customer { get; set; } = null!;
    }
}

