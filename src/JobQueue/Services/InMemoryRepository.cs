using JobQueue.Interfaces;
using JobQueue.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobQueue.Services
{
    public class InMemoryRepository<TEnt> : IJobRepository<TEnt> where TEnt : BaseEntity
    {
        private readonly ConcurrentDictionary<Guid, TEnt> _store;
        public InMemoryRepository(ConcurrentDictionary<Guid, TEnt> store)
        {
            _store = store;
        }

        public Task DeleteAsync(Guid id)
        {
            _store.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        public Task<TEnt> FindByIdAsync(Guid id)
        {
            if (!_store.TryGetValue(id, out TEnt value))
            {
                throw new KeyNotFoundException();
            }

            return Task.FromResult(value);
        }

        public Task<IEnumerable<TEnt>> ListAsync()
        {
            return Task.FromResult(_store.Values.AsEnumerable());
        }

        public Task SaveAsync(TEnt item)
        {
            _store.AddOrUpdate(item.Id, item, (key, value) => item);
            return Task.CompletedTask;
        }
    }
}
