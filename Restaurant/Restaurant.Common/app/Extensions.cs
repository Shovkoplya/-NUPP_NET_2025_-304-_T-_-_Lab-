namespace Restaurant.Common
{
    public static class Extensions
    {
        // метод розширення для Customer
        public static string GetContactInfo(this Customer customer)
        {
            return $"{customer.FullName} | {customer.PhoneNumber}";
        }
    }
}
