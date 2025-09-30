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
    }
}
