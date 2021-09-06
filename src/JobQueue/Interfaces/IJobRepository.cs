using JobQueue.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobQueue.Interfaces
{
    public interface IJobRepository<TEnt> where TEnt : BaseEntity
	{
        Task<IEnumerable<TEnt>> ListAsync();
        Task DeleteAsync(Guid id);
        Task SaveAsync(TEnt item);
        Task<TEnt> FindByIdAsync(Guid id);
    }
}
