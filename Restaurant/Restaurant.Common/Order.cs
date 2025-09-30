using System;
using System.Collections.Generic;

namespace Restaurant.Common
{
    // делегат
    public delegate void OrderCreatedHandler(Order order);

    public class Order
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<Dish> Dishes { get; set; } = new List<Dish>();
        public double TotalPrice { get; set; }

        // подія
        public event OrderCreatedHandler? OnOrderCreated;

        // конструктор
        public Order(List<Dish> dishes)
        {
            Id = Guid.NewGuid();
            OrderDate = DateTime.Now;
            Dishes = dishes;
            TotalPrice = CalculateTotal();

            // викликаємо подію після створення
            OnOrderCreated?.Invoke(this);
        }

        // метод
        private double CalculateTotal()
        {
            double total = 0;
            foreach (var dish in Dishes)
            {
                total += dish.Price;
            }
            return total;
        }

        public string GetSummary()
        {
            return $"Замовлення {Id} від {OrderDate}, сума: {TotalPrice} грн";
        }
    }
}
