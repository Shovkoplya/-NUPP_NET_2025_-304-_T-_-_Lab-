using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Restaurant.Common;

public static class Parallell
{
    // Створює count елементів паралельно, використовуючи factory для створення об'єктів
    public static async Task<int> CreateManyAsync<T>(CrudServiceAsync<T> service, int count, Func<T> factory) where T : IHasId
    {
        if (service == null) throw new ArgumentNullException(nameof(service));
        if (factory == null) throw new ArgumentNullException(nameof(factory));
        if (count <= 0) return 0;

        var tasks = new List<Task<bool>>(count);

        for (int i = 0; i < count; i++)
        {
            tasks.Add(Task.Run(async () => await service.CreateAsync(factory())));
        }

        var results = await Task.WhenAll(tasks);
        return results.Count(r => r);
    }
}
