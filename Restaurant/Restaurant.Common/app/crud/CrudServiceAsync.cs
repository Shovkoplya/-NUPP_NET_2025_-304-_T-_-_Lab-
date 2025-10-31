using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Restaurant.Common
{
    public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : IHasId
    {
        private readonly List<T> _items = new();
        public string FilePath { get; }
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public CrudServiceAsync(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

            if (File.Exists(FilePath))
            {
                try
                {
                    var jsonData = File.ReadAllText(FilePath);
                    var loaded = JsonSerializer.Deserialize<List<T>>(jsonData);
                    if (loaded != null && loaded.Count > 0)
                    {
                        _items.AddRange(loaded);
                    }
                }
                catch (Exception)
                {
                    // ignore malformed file, start with empty list
                }
            }
        }

        public async Task<bool> CreateAsync(T element)
        {
            if (element == null) 
                return false;

            await _semaphore.WaitAsync();
            try
            {
                _items.Add(element);
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<T> ReadAsync(Guid id)
        {
            await _semaphore.WaitAsync();
            try
            {
                // return default(T) (possibly null for reference types) when not found
                return _items.FirstOrDefault(i => i.Id == id)!;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                // return a snapshot
                return _items.ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            if (page <= 0 || amount <= 0)
            {
                return Enumerable.Empty<T>();
            }

            await _semaphore.WaitAsync();
            try
            {
                return _items.Skip((page - 1) * amount).Take(amount).ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> UpdateAsync(T element)
        {
            if (element == null) 
                return false;

            await _semaphore.WaitAsync();
            try
            {
                var index = _items.FindIndex(i => i.Id == element.Id);
                if (index == -1) return false;

                _items[index] = element;
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> RemoveAsync(T element)
        {
            if (element == null) 
                return false;

            await _semaphore.WaitAsync();
            try
            {
                return _items.Remove(element);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> SaveAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                var jsonData = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
                // ensure target directory exists
                var dir = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                await File.WriteAllTextAsync(FilePath, jsonData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
