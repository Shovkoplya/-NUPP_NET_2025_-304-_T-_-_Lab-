namespace Restaurant.Common
{
    public class Pizza : Dish
    {
        public int SizeCm { get; set; }
        public string DoughType { get; set; }
        public bool ExtraCheese { get; set; }

        // конструктор
        public Pizza(string name, double price, int sizeCm, string doughType, bool extraCheese)
            : base(name, price)
        {
            SizeCm = sizeCm;
            DoughType = doughType;
            ExtraCheese = extraCheese;
        }

        public override string GetInfo()
        {
            return $"{Name} (Піца {SizeCm} см, тісто: {DoughType}, дод. сир: {ExtraCheese}) - {Price} грн";
        }

    // Генерує приклад Pizza
    public new static Pizza GenerateSample()
        {
            var rnd = Random.Shared;
            var sizes = new[] { 26, 30, 33, 40 };
            var doughs = new[] { "Thin", "Classic", "WholeWheat" };
            var size = sizes[rnd.Next(sizes.Length)];
            var dough = doughs[rnd.Next(doughs.Length)];
            var price = Math.Round(rnd.NextDouble() * 500 + 80, 2);
            return new Pizza($"Pizza {rnd.Next(100, 999)}", price, size, dough, rnd.Next(0, 2) == 1);
        }

        // CreateNew wrapper
        public new static Pizza CreateNew() => GenerateSample();
    }
}
