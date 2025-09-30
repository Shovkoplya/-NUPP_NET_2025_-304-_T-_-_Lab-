using System;
using System.Collections.Generic;
using System.Reflection;

namespace Restaurant.Common
{
    public class CrudService<T> : ICrudService<T> where T : class
    {
        private readonly Dictionary<Guid, T> _storage = new();

        // метод Create
        public void Create(T element)
        {
            var id = GetId(element);
            if (!_storage.ContainsKey(id))
                _storage.Add(id, element);
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
        private Guid GetId(T element)
        {
            var prop = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);

            return (Guid)prop.GetValue(element)!;
        }
    }
}
