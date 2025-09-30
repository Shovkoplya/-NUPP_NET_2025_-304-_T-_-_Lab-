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
    }
}
