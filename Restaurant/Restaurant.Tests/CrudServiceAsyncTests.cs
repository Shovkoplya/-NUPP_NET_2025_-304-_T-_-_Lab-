using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Restaurant.Common;
using Xunit;

namespace Restaurant.Tests
{
    public class CrudServiceAsyncTests : IDisposable
    {
        private readonly string _tempFile;

        public CrudServiceAsyncTests()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"crud_async_test_{Guid.NewGuid()}.json");
        }

        [Fact]
        public async Task CreateReadUpdateRemove_SaveFlow()
        {
            var service = new CrudServiceAsync<Dish>(_tempFile);

            var d1 = Dish.GenerateSample();
            var created = await service.CreateAsync(d1);
            Assert.True(created);

            var read = await service.ReadAsync(d1.Id);
            Assert.NotNull(read);
            Assert.Equal(d1.Id, read.Id);

            d1.Price = d1.Price + 10;
            var updated = await service.UpdateAsync(d1);
            Assert.True(updated);

            var read2 = await service.ReadAsync(d1.Id);
            Assert.Equal(d1.Price, read2.Price);

            var removed = await service.RemoveAsync(d1);
            Assert.True(removed);

            var read3 = await service.ReadAsync(d1.Id);
            Assert.Null(read3);

            // save
            await service.CreateAsync(Dish.GenerateSample());
            var ok = await service.SaveAsync();
            Assert.True(ok);
            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public async Task ReadAllAndPagination()
        {
            var service = new CrudServiceAsync<Dish>(_tempFile);
            for (int i = 0; i < 25; i++) await service.CreateAsync(Dish.GenerateSample());

            var all = (await service.ReadAllAsync()).ToList();
            Assert.Equal(25, all.Count);

            var page2 = (await service.ReadAllAsync(2, 10)).ToList();
            Assert.Equal(10, page2.Count);

            var page3 = (await service.ReadAllAsync(3, 10)).ToList();
            Assert.Equal(5, page3.Count);
        }

        [Fact]
        public async Task ConcurrentCreates_AreThreadSafe()
        {
            var service = new CrudServiceAsync<Dish>(_tempFile);
            var tasks = Enumerable.Range(0, 200).Select(i => service.CreateAsync(Dish.GenerateSample())).ToArray();
            await Task.WhenAll(tasks);

            var all = (await service.ReadAllAsync()).ToList();
            Assert.Equal(200, all.Count);
        }

        public void Dispose()
        {
            try { if (File.Exists(_tempFile)) File.Delete(_tempFile); } catch { }
        }
    }
}
