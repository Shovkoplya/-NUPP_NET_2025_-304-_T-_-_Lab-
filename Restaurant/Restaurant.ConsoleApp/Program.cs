using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Restaurant.Common;

class Program
{
    static async Task Main(string[] args)
    {
        // Задаємо шлях для збереження
        var filePath = "data/dishes_async.json";

        // створюємо асинхронний CRUD сервіс для Dish
        var dishService = new CrudServiceAsync<Dish>(filePath);

        // Паралельно створюємо 1000 об'єктів через Parallell helper
        var created = await Parallell.CreateManyAsync(dishService, 1000, () => Dish.CreateNew());

        Console.WriteLine($"Створено елементів: {created}");

        // Отримаємо всі елементи для аналізу
        var all = (await dishService.ReadAllAsync()).ToList();

        // Аналіз чисельних властивостей через рефлексію
        var numericProps = typeof(Dish).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsNumericType(p.PropertyType)).ToList();

        foreach (var prop in numericProps)
        {
            var values = all.Select(o => Convert.ToDouble(prop.GetValue(o) ?? 0)).ToList();
            if (values.Count == 0) continue;
            var min = values.Min();
            var max = values.Max();
            var avg = values.Average();
            Console.WriteLine($"{prop.Name}: min={min}, max={max}, avg={avg:F2}");
        }

        // Зберігаємо колекцію у файл
        var ok = await dishService.SaveAsync();
        Console.WriteLine($"SaveAsync returned: {ok}");
        
        // Запускаємо приклади примітивів синхронізації
        SyncExamples.RunAll();
    }

    static bool IsNumericType(Type type)
    {
        Type t = Nullable.GetUnderlyingType(type) ?? type;
        return t == typeof(byte) || t == typeof(sbyte) || t == typeof(short) || t == typeof(ushort)
            || t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong)
            || t == typeof(float) || t == typeof(double) || t == typeof(decimal);
    }
}
