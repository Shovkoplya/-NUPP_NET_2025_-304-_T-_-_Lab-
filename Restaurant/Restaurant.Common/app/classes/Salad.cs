namespace Restaurant.Common
{
    public class Salad : Dish
    {
        public bool IsVegetarian { get; set; }
        public int Calories { get; set; }
        public string Dressing { get; set; }

        // конструктор (викликає базовий)
        public Salad(string name, double price, bool isVegetarian, int calories, string dressing)
            : base(name, price)
        {
            IsVegetarian = isVegetarian;
            Calories = calories;
            Dressing = dressing;
        }

        // метод (override)
        public override string GetInfo()
        {
            return $"{Name} (Салат, {Calories} ккал) - {Price} грн, Заправка: {Dressing}";
        }

    // Генерує приклад Salad
    public new static Salad GenerateSample()
        {
            var rnd = Random.Shared;
            var dressings = new[] { "Olive", "Yogurt", "Vinaigrette", "Caesar" };
            var calories = rnd.Next(120, 600);
            var price = Math.Round(rnd.NextDouble() * 200 + 30, 2);
            return new Salad($"Salad {rnd.Next(100,999)}", price, rnd.Next(0,2) == 1, calories, dressings[rnd.Next(dressings.Length)]);
        }

        // CreateNew wrapper
        public new static Salad CreateNew() => GenerateSample();
    }
}
