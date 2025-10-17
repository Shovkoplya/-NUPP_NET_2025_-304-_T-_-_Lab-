using System;

namespace Restaurant.Common
{
    public class Dish : IHasId
    {
        // статичне поле
        public static int TotalDishesCreated;

        // властивості
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        // конструктор
        public Dish(string name, double price)
        {
            Id = Guid.NewGuid();
            Name = name;
            Price = price;
            TotalDishesCreated++;
        }

        // статичний конструктор
        static Dish()
        {
            TotalDishesCreated = 0;
        }

        // метод
        public virtual string GetInfo()
        {
            return $"{Name} - {Price} грн";
        }

        // статичний метод
        public static int GetTotalDishesCount()
        {
            return TotalDishesCreated;
        }

        // Генерує приклад Dish
        public static Dish GenerateSample()
        {
            var rnd = Random.Shared;
            var price = Math.Round(rnd.NextDouble() * 400 + 50, 2); // 50..450
            return new Dish($"Dish {rnd.Next(100, 999)}", price);
        }

        // CreateNew wrapper
        public static Dish CreateNew() => GenerateSample();
    }
}
