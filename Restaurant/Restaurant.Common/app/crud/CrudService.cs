using System.Reflection;
using System.Text.Json;

namespace Restaurant.Common
{
    public class CrudService<T> : ICrudService<T> where T : class
    {
        private readonly Dictionary<Guid, T> _storage = [];

        // метод Create
        public void Create(T element)
        {
            var id = GetId(element);
            _storage.TryAdd(id, element);
        }

        // метод Read
        public T? Read(Guid id)
        {
            _storage.TryGetValue(id, out var element);
            return element;
        }

        // метод ReadAll
        public IEnumerable<T> ReadAll()
        {
            return _storage.Values;
        }

        // метод Update
        public void Update(T element)
        {
            var id = GetId(element);
            if (_storage.ContainsKey(id))
            {
                _storage[id] = element;
            }
        }

        // метод Remove
        public void Remove(T element)
        {
            var id = GetId(element);
            _storage.Remove(id);
        }

        // приватний хелпер: отримати Id через рефлексію
        private static Guid GetId(T element)
        {
            var prop = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);

            return (Guid)prop.GetValue(element)!;
        }

        // метод Save - зберігає дані у JSON файл
        public void Save(string filePath)
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(_storage, jsonOptions);
            File.WriteAllText(filePath, jsonString);
        }

        // метод Load - завантажує дані з JSON файлу
        public void Load(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            var jsonString = File.ReadAllText(filePath);
            var loadedData = JsonSerializer.Deserialize<Dictionary<Guid, T>>(jsonString);
            
            _storage.Clear();
            foreach (var item in loadedData!)
            {
                _storage.Add(item.Key, item.Value);
            }
        }
    }
}
