namespace Restaurant.Common
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int LoyaltyPoints { get; set; }
    }
}
