using System;
using System.Linq;
using System.Threading.Tasks;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repositories;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        Console.WriteLine("Restaurant Management - Database Demo");
        Console.WriteLine("Repository Pattern + PostgreSQL");

        try
        {
            // Створюємо контекст бази даних та Unit of Work
            using var context = new RestaurantContext();
            using var unitOfWork = new UnitOfWork(context);

            Console.WriteLine("=== Підключення до бази даних ===\n");

            // 1. Демонстрація роботи з клієнтами
            await DemoCustomersAsync(unitOfWork);

            // 2. Демонстрація роботи зі стравами
            await DemoDishesAsync(unitOfWork);

            // 3. Демонстрація роботи із замовленнями
            await DemoOrdersAsync(unitOfWork);

            // 4. Статистика
            await ShowStatisticsAsync(unitOfWork);

            Console.WriteLine("Демонстрація завершена успішно!");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Помилка: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Деталі: {ex.InnerException.Message}");
            }
            Console.ResetColor();
        }
    }

    static async Task DemoCustomersAsync(IUnitOfWork unitOfWork)
    {
        Console.WriteLine("1. Робота з клієнтами");

        // Показати всіх клієнтів
        var customers = await unitOfWork.Customers.GetAllAsync();
        var customerList = customers.ToList();
        
        Console.WriteLine($"✅ Всього клієнтів у базі: {customerList.Count}");
        foreach (var customer in customerList)
        {
            Console.WriteLine($"   - {customer.FullName} ({customer.PhoneNumber})");
            Console.WriteLine($"     Бали лояльності: {customer.LoyaltyPoints}");
        }

        // Отримати клієнта з профілем
        if (customerList.Any())
        {
            var firstCustomerId = customerList.First().Id;
            var customerWithProfile = await unitOfWork.Customers.GetWithProfileAsync(firstCustomerId);
            
            if (customerWithProfile?.Profile != null)
            {
                Console.WriteLine($"\n📧 Профіль клієнта '{customerWithProfile.FullName}':");
                Console.WriteLine($"   Email: {customerWithProfile.Profile.Email}");
                Console.WriteLine($"   Адреса: {customerWithProfile.Profile.Address}");
                Console.WriteLine($"   Спосіб оплати: {customerWithProfile.Profile.PreferredPaymentMethod}");
            }
        }

        // Топ клієнтів за лояльністю
        var topCustomers = await unitOfWork.Customers.GetTopLoyalCustomersAsync(3);
        Console.WriteLine($"\n🏆 Топ-3 клієнти за балами лояльності:");
        foreach (var customer in topCustomers)
        {
            Console.WriteLine($"   {customer.FullName}: {customer.LoyaltyPoints} балів");
        }
    }

    static async Task DemoDishesAsync(IUnitOfWork unitOfWork)
    {
        Console.WriteLine("2. Робота зі стравами (меню ресторану)");

        // Показати всі доступні страви
        var dishes = await unitOfWork.Dishes.GetAvailableDishesAsync();
        var dishList = dishes.ToList();
        
        Console.WriteLine($"✅ Доступно страв у меню: {dishList.Count}");
        
        foreach (var dish in dishList)
        {
            var dishType = dish.GetType().Name.Replace("Model", "");
            Console.WriteLine($"   [{dishType,-6}] {dish.Name,-20} {dish.Price,6:F2} грн");
        }

        // Показати тільки піци
        var pizzas = await unitOfWork.Dishes.GetAllPizzasAsync();
        Console.WriteLine($"\n🍕 Піци в меню ({pizzas.Count()}):");
        foreach (var pizza in pizzas)
        {
            Console.WriteLine($"   {pizza.Name} - {pizza.SizeCm}см, {pizza.DoughType}");
            Console.WriteLine($"   Ціна: {pizza.Price} грн, Екстра сир: {(pizza.ExtraCheese ? "Так" : "Ні")}");
        }

        // Показати вегетаріанські салати
        var vegSalads = await unitOfWork.Dishes.GetVegetarianSaladsAsync();
        Console.WriteLine($"\n🥗 Вегетаріанські салати ({vegSalads.Count()}):");
        foreach (var salad in vegSalads)
        {
            Console.WriteLine($"   {salad.Name} - {salad.Calories} ккал");
            Console.WriteLine($"   Ціна: {salad.Price} грн, Заправка: {salad.Dressing}");
        }
    }

    static async Task DemoOrdersAsync(IUnitOfWork unitOfWork)
    {
        Console.WriteLine("3. Робота із замовленнями");

        // Показати останні замовлення
        var recentOrders = await unitOfWork.Orders.GetRecentOrdersAsync(5);
        var orderList = recentOrders.ToList();
        
        Console.WriteLine($"✅ Останні замовлення ({orderList.Count}):");
        foreach (var order in orderList)
        {
            Console.WriteLine($"\n   Замовлення від {order.OrderDate:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"   Клієнт: {order.Customer.FullName}");
            Console.WriteLine($"   Статус: {order.Status}");
            Console.WriteLine($"   Сума: {order.TotalPrice:F2} грн");
            
            if (!string.IsNullOrEmpty(order.DeliveryAddress))
            {
                Console.WriteLine($"   Доставка: {order.DeliveryAddress}");
            }
        }

        // Показати замовлення за статусом
        var pendingOrders = await unitOfWork.Orders.GetOrdersByStatusAsync("Pending");
        Console.WriteLine($"\n⏳ Замовлень в обробці: {pendingOrders.Count()}");
    }

    static async Task ShowStatisticsAsync(IUnitOfWork unitOfWork)
    {
        Console.WriteLine("4. Статистика та аналітика");

        // Статистика по стравах
        var allDishes = await unitOfWork.Dishes.GetAllAsync();
        var dishesList = allDishes.ToList();
        
        if (dishesList.Any())
        {
            var minPrice = dishesList.Min(d => d.Price);
            var maxPrice = dishesList.Max(d => d.Price);
            var avgPrice = dishesList.Average(d => d.Price);
            
            Console.WriteLine("📊 Статистика по стравах:");
            Console.WriteLine($"   Всього страв: {dishesList.Count}");
            Console.WriteLine($"   Мінімальна ціна: {minPrice:F2} грн");
            Console.WriteLine($"   Максимальна ціна: {maxPrice:F2} грн");
            Console.WriteLine($"   Середня ціна: {avgPrice:F2} грн");
        }

        // Статистика по замовленнях
        var allOrders = await unitOfWork.Orders.GetAllAsync();
        var ordersList = allOrders.ToList();
        
        if (ordersList.Any())
        {
            var totalRevenue = ordersList.Sum(o => o.TotalPrice);
            var avgOrderValue = ordersList.Average(o => o.TotalPrice);
            var completedCount = ordersList.Count(o => o.Status == "Completed");
            var pendingCount = ordersList.Count(o => o.Status == "Pending");
            
            Console.WriteLine("\n📊 Статистика по замовленнях:");
            Console.WriteLine($"   Всього замовлень: {ordersList.Count}");
            Console.WriteLine($"   Загальний дохід: {totalRevenue:F2} грн");
            Console.WriteLine($"   Середній чек: {avgOrderValue:F2} грн");
            Console.WriteLine($"   Завершено: {completedCount}");
            Console.WriteLine($"   В обробці: {pendingCount}");
        }

        // Статистика по клієнтах
        var allCustomers = await unitOfWork.Customers.GetAllAsync();
        var customersList = allCustomers.ToList();
        
        if (customersList.Any())
        {
            var totalLoyaltyPoints = customersList.Sum(c => c.LoyaltyPoints);
            var avgLoyaltyPoints = customersList.Average(c => c.LoyaltyPoints);
            
            Console.WriteLine("\n📊 Статистика по клієнтах:");
            Console.WriteLine($"   Всього клієнтів: {customersList.Count}");
            Console.WriteLine($"   Всього балів лояльності: {totalLoyaltyPoints}");
            Console.WriteLine($"   Середньо балів на клієнта: {avgLoyaltyPoints:F2}");
        }
    }
}
