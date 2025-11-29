namespace Restaurant.REST.Models
{
    /// <summary>
    /// DTO для повної інформації про клієнта
    /// </summary>
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int LoyaltyPoints { get; set; }
        public CustomerProfileDto? Profile { get; set; }
    }

    /// <summary>
    /// DTO для створення нового клієнта
    /// </summary>
    public class CreateCustomerDto
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int LoyaltyPoints { get; set; } = 0;
    }

    /// <summary>
    /// DTO для оновлення даних клієнта
    /// </summary>
    public class UpdateCustomerDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public int? LoyaltyPoints { get; set; }
    }
}

