using System;
using Restaurant.Common;

class Program
{
    static void Main(string[] args)
    {
        // створюємо CRUD сервіс для Salad
        var saladService = new CrudService<Salad>();

        var salad1 = new Salad("Грецький", 120, true, 200, "Оливкова олія");
        var salad2 = new Salad("Цезар", 150, false, 350, "Майонез");

        // Create
        saladService.Create(salad1);
        saladService.Create(salad2);

        Console.WriteLine("Після створення:");
        foreach (var s in saladService.ReadAll())
        {
            Console.WriteLine(s.GetInfo());
        }

        // Read
        var readSalad = saladService.Read(salad1.Id);
        Console.WriteLine($"\nЗнайдено по Id: {readSalad?.GetInfo()}");

        // Update
        salad1.Price = 135;
        saladService.Update(salad1);

        Console.WriteLine("\nПісля оновлення:");
        foreach (var s in saladService.ReadAll())
        {
            Console.WriteLine(s.GetInfo());
        }

        // Remove
        saladService.Remove(salad2);

        Console.WriteLine("\nПісля видалення:");
        foreach (var s in saladService.ReadAll())
        {
            Console.WriteLine(s.GetInfo());
        }
    }
}
