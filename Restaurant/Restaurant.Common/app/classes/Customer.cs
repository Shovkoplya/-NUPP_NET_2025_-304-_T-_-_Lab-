using System;

namespace Restaurant.Common
{
    public class Customer : IHasId
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int LoyaltyPoints { get; set; }

        // Генерує приклад об'єкта Customer з випадковими даними
        public static Customer GenerateSample()
        {
            var rnd = Random.Shared;
            return new Customer
            {
                Id = Guid.NewGuid(),
                FullName = $"Customer {rnd.Next(1000, 9999)}",
                PhoneNumber = $"+380{rnd.Next(10000000, 99999999)}",
                LoyaltyPoints = rnd.Next(0, 2000)
            };
        }

        // Сумісний CreateNew wrapper
        public static Customer CreateNew() => GenerateSample();
    }
}
