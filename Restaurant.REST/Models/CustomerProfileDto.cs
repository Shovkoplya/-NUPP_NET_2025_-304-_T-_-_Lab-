namespace Restaurant.REST.Models
{
    /// <summary>
    /// DTO для профілю клієнта
    /// </summary>
    public class CustomerProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PreferredPaymentMethod { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
    }

    /// <summary>
    /// DTO для створення профілю клієнта
    /// </summary>
    public class CreateCustomerProfileDto
    {
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PreferredPaymentMethod { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO для оновлення профілю клієнта
    /// </summary>
    public class UpdateCustomerProfileDto
    {
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? PreferredPaymentMethod { get; set; }
    }
}

