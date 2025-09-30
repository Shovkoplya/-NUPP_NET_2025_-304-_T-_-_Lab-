using System;

namespace Restaurant.Common
{
    public class Dish
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
    }
}
